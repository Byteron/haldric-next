using RelEcs;
using RelEcs.Godot;

public class DeathTrigger
{
    public Entity Entity { get; set; }

    public DeathTrigger(Entity entity)
    {
        Entity = entity;
    }
}

public class DeathTriggerSystem : ISystem
{
    public void Run(Commands commands)
    {
        var checkVictory = false;

        commands.Receive((DeathTrigger deathEvent) =>
        {
            var map = commands.GetElement<Map>();

            var coords = deathEvent.Entity.Get<Coords>();

            var locEntity = map.Locations.Get(coords.Cube());

            locEntity.Remove<HasUnit>();

            deathEvent.Entity.DespawnAndFree();

            checkVictory = true;
        });

        if (checkVictory) commands.Send(new CheckVictoryConditionTrigger());
    }
}