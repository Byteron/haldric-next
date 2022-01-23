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
        bool checkVictory = false;

        world.ForEach((ref DeathEvent deathEvent) =>
        {
            var map = world.GetResource<Map>();

            ref var coords = ref deathEvent.Entity.Get<Coords>();

            var locEntity = map.Locations.Get(coords.Cube());

            locEntity.Remove<HasUnit>();

            deathEvent.Entity.Despawn();

            checkVictory = true;
        });

        if (checkVictory)
        {
            world.Spawn().Add(new CheckVictoryConditionEvent());
        }
    }
}