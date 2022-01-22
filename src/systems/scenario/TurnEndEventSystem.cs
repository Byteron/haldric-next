using Bitron.Ecs;
using Godot;

public struct TurnEndEvent { }

public class TurnEndEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        world.ForEach((ref TurnEndEvent e) =>
        {
            var scenario = world.GetResource<Scenario>();
            scenario.EndTurn();

            if (scenario.HasRoundChanged())
            {
                world.Spawn().Add(new ChangeDaytimeEvent());
            }

            var sideEntity = scenario.GetCurrentSideEntity();

            var gold = sideEntity.Get<Gold>().Value;
            var side = sideEntity.Get<Side>();
            ref var playerId = ref sideEntity.Get<PlayerId>();

            world.ForEach((EcsEntity unitEntity, ref Side unitSide, ref Attribute<Actions> actions, ref Attribute<Moves> moves, ref Level level) =>
            {
                if (side.Value == unitSide.Value)
                {
                    actions.Restore();
                    moves.Restore();

                    if (unitEntity.Has<Suspended>())
                    {
                        unitEntity.Remove<Suspended>();
                    }

                    gold -= level.Value;
                }
            });

            world.ForEach((ref Village village, ref IsCapturedBySide villageSide) =>
            {
                if (side.Value == villageSide.Value)
                {
                    gold += village.List.Count;
                }
            });

            if (world.TryGetResource<TurnPanel>(out var turnPanel))
            {
                var localPlayer = world.GetResource<LocalPlayer>();

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

            world.ForEach((EcsEntity locEntity, ref HasBaseTerrain baseTerrain, ref HasUnit hasUnit) =>
            {

                var canHeal = baseTerrain.Entity.Has<Heals>();

                if (locEntity.Has<HasOverlayTerrain>())
                {
                    canHeal = canHeal || locEntity.Get<HasOverlayTerrain>().Entity.Has<Heals>();
                }

                var unitEntity = hasUnit.Entity;

                ref var health = ref unitEntity.Get<Attribute<Health>>();
                ref var unitSide = ref unitEntity.Get<Side>();

                if (canHeal && unitSide.Value == side.Value && !health.IsFull())
                {
                    var diff = Mathf.Min(health.GetDifference(), 8);

                    health.Increase(diff);

                    world.Spawn().Add(new SpawnFloatingLabelEvent(unitEntity.Get<Coords>().World() + Godot.Vector3.Up * 7f, diff.ToString(), new Godot.Color(0f, 1f, 0f)));
                }
            });

            sideEntity.Get<Gold>().Value = gold;
        });
    }
}