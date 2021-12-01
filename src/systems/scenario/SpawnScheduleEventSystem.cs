using Bitron.Ecs;
using Haldric.Wdk;
using Godot;

public struct SpawnScheduleEvent
{
    public string Id;
    public int Index;

    public SpawnScheduleEvent(string id, int index = 0)
    {
        Id = id;
        Index = index;
    }
}

public class SpawnScheduleEventSystem : IEcsSystem
{
    private Node3D _parent;

    public SpawnScheduleEventSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        var query = world.Query<SpawnScheduleEvent>().End();

        foreach (var eventId in query)
        {
            var eventEntity = world.Entity(eventId);
            ref var spawnEvent = ref eventEntity.Get<SpawnScheduleEvent>();

            var schedule = Data.Instance.Schedules[spawnEvent.Id].Instantiate<Schedule>();
            _parent.AddChild(schedule);

            schedule.Set(spawnEvent.Index);

            world.AddResource(schedule);
        }
    }
}
