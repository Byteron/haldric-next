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

            ref var deathEvent = ref query.Get<DeathEvent>(eventEntityId);

            ref var coords = ref deathEvent.Entity.Get<Coords>();

            var locEntity = map.Locations.Get(coords.Cube);

            locEntity.Remove<HasUnit>();

            deathEvent.Entity.Despawn();
        }
    }
}