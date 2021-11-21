using Bitron.Ecs;
using Godot;

public struct TurnEndEvent { }

public class TurnEndEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<TurnEndEvent>().End();
        var unitQuery = world.Query<Side>().Inc<Attribute<Actions>>().Inc<Level>().End();
        var locsWithCapturedVillagesQuery = world.Query<Village>().Inc<IsCapturedBySide>().End();
        var locWithUnitQuery = world.Query<HasBaseTerrain>().Inc<HasUnit>().End();

        foreach (var eventEntityId in eventQuery)
        {
            var scenario = world.GetResource<Scenario>();
            scenario.EndTurn();

            if (scenario.HasRoundChanged())
            {
                world.Spawn().Add(new ChangeDaytimeEvent());
            }

            var player = scenario.GetCurrentPlayerEntity();

            ref var gold = ref player.Get<Gold>();

            foreach (var unitEntityId in unitQuery)
            {
                var unitEntity = world.Entity(unitEntityId);

                ref var side = ref unitEntity.Get<Side>();

                if (side.Value == scenario.CurrentPlayer)
                {
                    ref var actions = ref unitEntity.Get<Attribute<Actions>>();
                    ref var moves = ref unitEntity.Get<Attribute<Moves>>();

                    actions.Restore();
                    moves.Restore();

                    if (unitEntity.Has<Suspended>())
                    {
                        unitEntity.Remove<Suspended>();
                    }

                    ref var level = ref unitEntity.Get<Level>();

                    gold.Value -= level.Value;
                }
            }

            foreach (var locEntityId in locsWithCapturedVillagesQuery)
            {
                var locEntity = world.Entity(locEntityId);

                ref var village = ref locEntity.Get<Village>();
                ref var side = ref locEntity.Get<IsCapturedBySide>();

                if (scenario.CurrentPlayer == side.Value)
                {
                    gold.Value += village.List.Count;
                }
            }

            var turnPanel = world.GetResource<TurnPanel>();
            var localPlayer = world.GetResource<LocalPlayer>();

            if (scenario.CurrentPlayer == localPlayer.Side)
            {
                Sfx.Instance.Play("TurnBell");
                turnPanel.EndTurnButton.Disabled = false;
            }
            else
            {
                turnPanel.EndTurnButton.Disabled = true;
            }

            foreach (var locEntityId in locWithUnitQuery)
            {
                var locEntity = world.Entity(locEntityId);

                var canHeal = locEntity.Get<HasBaseTerrain>().Entity.Has<Heals>();

                if (locEntity.Has<HasOverlayTerrain>())
                {
                    canHeal = canHeal || locEntity.Get<HasOverlayTerrain>().Entity.Has<Heals>();
                }

                var unitEntity = locEntity.Get<HasUnit>().Entity;

                ref var health = ref unitEntity.Get<Attribute<Health>>();
                ref var side = ref unitEntity.Get<Side>();

                if (canHeal && side.Value == scenario.CurrentPlayer && !health.IsFull())
                {
                    var diff = Mathf.Min(health.GetDifference(), 8);

                    health.Increase(diff);

                    world.Spawn().Add(new SpawnFloatingLabelEvent(unitEntity.Get<Coords>().World() + Godot.Vector3.Up * 7f, diff.ToString(), new Godot.Color(0f, 1f, 0f)));
                }
            }
        }
    }
}