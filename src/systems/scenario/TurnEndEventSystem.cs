using Bitron.Ecs;
using Godot;

public struct TurnEndEvent { }

public class TurnEndEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<TurnEndEvent>().End();
        var unitQuery = world.Query<Team>().Inc<Attribute<Actions>>().End();
        var locWithUnitQuery = world.Query<HasBaseTerrain>().Inc<HasUnit>().End();

        foreach (var eventEntityId in eventQuery)
        {
            var scenario = world.GetResource<Scenario>();

            scenario.EndTurn();
            
            Godot.GD.Print("Player: " + scenario.CurrentPlayer);
            Godot.GD.Print("Units: ", unitQuery.GetEntitiesCount());

            foreach (var unitEntityId in unitQuery)
            {
                var team = unitQuery.Get<Team>(unitEntityId);
                if (team.Value == scenario.CurrentPlayer)
                {
                    ref var actions = ref unitQuery.Get<Attribute<Actions>>(unitEntityId);
                    actions.Restore();
                }
            }

            var hudView = world.GetResource<HUDView>();

            foreach (var locEntityId in locWithUnitQuery)
            {
                var locEntity = world.Entity(locEntityId);

                var canHeal = locEntity.Get<HasBaseTerrain>().Entity.Has<Heals>();

                if (locEntity.Has<HasOverlayTerrain>())
                {
                    canHeal = canHeal || locEntity.Get<HasOverlayTerrain>().Entity.Has<Heals>();
                }
                
                var unitEntity = locWithUnitQuery.Get<HasUnit>(locEntityId).Entity;

                ref var health = ref unitEntity.Get<Attribute<Health>>();
                ref var team = ref unitEntity.Get<Team>();

                if (canHeal && team.Value == scenario.CurrentPlayer && !health.IsFull())
                {
                    var diff = Mathf.Min(health.GetDifference(), 8);

                    health.Increase(diff);

                    hudView.SpawnFloatingLabel(unitEntity.Get<Coords>().World + Godot.Vector3.Up * 7f, diff.ToString(), new Godot.Color(0f, 1f, 0f));
                }
            }
        }
    }
}