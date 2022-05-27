using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;

public class MissTrigger
{
    public Entity TargetEntity { get; }

    public MissTrigger(Entity targetEntity)
    {
        TargetEntity = targetEntity;
    }
}

public class MissTriggerSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((MissTrigger missEvent) =>
        {
            var position = missEvent.TargetEntity.Get<Coords>().World() + Vector3.Up * 5f;
            var spawnLabelEvent = new SpawnFloatingLabelEvent(position, "Miss!", new Color(0.7f, 0.7f, 0.7f));

            commands.Send(spawnLabelEvent);
        });
    }
}