using RelEcs;
using RelEcs.Godot;

public struct DeathEvent
{
    public Entity Entity { get; set; }

    public DeathEvent(Entity entity)
    {
        Entity = entity;
    }
}

public class DeathEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        bool checkVictory = false;

        commands.Receive((DeathEvent deathEvent) =>
        {
            var map = commands.GetElement<Map>();

            ref var coords = ref deathEvent.Entity.Get<Coords>();

            var locEntity = map.Locations.Get(coords.Cube());

            locEntity.Remove<HasUnit>();

            deathEvent.Entity.Despawn();

            checkVictory = true;
        });

        if (checkVictory)
        {
            commands.Send(new CheckVictoryConditionEvent());
        }
    }
}