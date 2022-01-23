using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public struct MissEvent
{
    public EcsEntity TargetEntity { get; set; }

    public MissEvent(EcsEntity targetEntity)
    {
        TargetEntity = targetEntity;
    }
}

public class MissEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        world.ForEach((ref MissEvent missEvent) =>
        {
            var targetEntity = missEvent.TargetEntity;

            var position = missEvent.TargetEntity.Get<Coords>().World() + Vector3.Up * 5f;
            var spawnLabelEvent = new SpawnFloatingLabelEvent(position, "Miss!", new Color(0.7f, 0.7f, 0.7f));

            world.Spawn().Add(spawnLabelEvent);
        });
    }
}