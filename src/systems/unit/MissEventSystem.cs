using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;

public class MissEvent
{
    public Entity TargetEntity { get; set; }

    public MissEvent(Entity targetEntity)
    {
        TargetEntity = targetEntity;
    }
}

public class MissEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((MissEvent missEvent) =>
        {
            var targetEntity = missEvent.TargetEntity;

            var position = missEvent.TargetEntity.Get<Coords>().World() + Vector3.Up * 5f;
            var spawnLabelEvent = new SpawnFloatingLabelEvent(position, "Miss!", new Color(0.7f, 0.7f, 0.7f));

            commands.Send(spawnLabelEvent);
        });
    }
}