using RelEcs;
using RelEcs.Godot;
using Godot;

public class TurnEndTrigger { }

public class TurnEndTriggerSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((TurnEndTrigger _) =>
        {
            var scenario = commands.GetElement<Scenario>();
            scenario.EndTurn();

            if (scenario.HasRoundChanged()) commands.Send(new ChangeDaytimeTrigger());

            var sideEntity = scenario.GetCurrentSideEntity();

            var gold = sideEntity.Get<Gold>().Value;
            var side = sideEntity.Get<Side>();
            var playerId = sideEntity.Get<PlayerId>();

            var unitQuery = commands.Query<Entity, Side, Attribute<Actions>, Attribute<Moves>, Level>();
            foreach (var (unitEntity, unitSide, actions, moves, level) in unitQuery)
            {
                if (side.Value != unitSide.Value) continue;
                
                actions.Restore();
                moves.Restore();

                if (unitEntity.Has<Suspended>())
                {
                    unitEntity.Remove<Suspended>();
                }

                gold -= level.Value;
            }

            foreach (var (village, villageSide) in commands.Query<Village, IsCapturedBySide>())
            {
                if (side.Value == villageSide.Value)
                {
                    gold += village.List.Count;
                }
            }

            if (commands.TryGetElement<TurnPanel>(out var turnPanel))
            {
                var localPlayer = commands.GetElement<LocalPlayer>();

                if (playerId.Value == localPlayer.Id)
                {
                    Sfx.Instance.Play("TurnBell");
                    turnPanel.EndTurnButton.Disabled = false;
                }
                else
                {
                    turnPanel.EndTurnButton.Disabled = true;
                }
            }

            var locQuery = commands.Query<Entity, HasBaseTerrain, HasUnit>();

            foreach (var (locEntity, baseTerrain, hasUnit) in locQuery)
            {
                var canHeal = baseTerrain.Entity.Has<Heals>();

                if (locEntity.Has<HasOverlayTerrain>())
                {
                    canHeal = canHeal || locEntity.Get<HasOverlayTerrain>().Entity.Has<Heals>();
                }

                var unitEntity = hasUnit.Entity;

                var health = unitEntity.Get<Attribute<Health>>();
                var unitSide = unitEntity.Get<Side>();

                if (!canHeal || unitSide.Value != side.Value || health.IsFull()) continue;
                
                var diff = Mathf.Min(health.GetDifference(), 8);

                health.Increase(diff);

                commands.Send(new SpawnFloatingLabelEvent(unitEntity.Get<Coords>().World() + Godot.Vector3.Up * 7f, diff.ToString(), new Godot.Color(0f, 1f, 0f)));
            }

            sideEntity.Get<Gold>().Value = gold;
        });
    }
}