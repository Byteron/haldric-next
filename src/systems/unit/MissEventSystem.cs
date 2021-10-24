using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public struct MissEvent
{
    public EcsEntity TargetEntity;

    public MissEvent(EcsEntity targetEntity)
    {
        TargetEntity = targetEntity;
    }
}

public class MissEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<MissEvent>().End();

        foreach (var eventEntityId in query)
        {
            ref var missEvent = ref query.Get<MissEvent>(eventEntityId);

            var targetEntity = missEvent.TargetEntity;

            var position = missEvent.TargetEntity.Get<Coords>().World + Vector3.Up * 5f;
            var spawnLabelEvent = new SpawnFloatingLabelEvent(position, "Miss!", new Color(0.7f, 0.7f, 0.7f));

            Main.Instance.World.Spawn().Add(spawnLabelEvent);
        }
    }
}