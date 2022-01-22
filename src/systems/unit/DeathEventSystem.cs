using Bitron.Ecs;

public struct DeathEvent
{
    public EcsEntity Entity { get; set; }

    public DeathEvent(EcsEntity entity)
    {
        Entity = entity;
    }
}

public class DeathEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<DeathEvent>().End();

        foreach (var eventEntityId in query)
        {
            var map = world.GetResource<Map>();

            ref var deathEvent = ref world.Entity(eventEntityId).Get<DeathEvent>();

            ref var coords = ref deathEvent.Entity.Get<Coords>();

            var locEntity = map.Locations.Get(coords.Cube());

            locEntity.Remove<HasUnit>();

            deathEvent.Entity.Despawn();
        }

        if (query.GetEntityCount() > 0)
        {
            world.Spawn().Add(new CheckVictoryConditionEvent());
        }
    }
}