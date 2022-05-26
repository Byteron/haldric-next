using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;
using Godot;

public class SpawnScheduleEvent
{
    public string Id;
    public int Index;

    public SpawnScheduleEvent()
    {
    }
    
    public SpawnScheduleEvent(string id, int index = 0)
    {
        Id = id;
        Index = index;
    }
}

public class SpawnScheduleEventSystem : ISystem
{
     Node3D _parent;

    public SpawnScheduleEventSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(Commands commands)
    {
        commands.Receive((SpawnScheduleEvent spawnEvent) =>
        {
            var schedule = Data.Instance.Schedules[spawnEvent.Id].Instantiate<Schedule>();
            _parent.AddChild(schedule);

            schedule.Set(spawnEvent.Index);

            commands.AddElement(schedule);
        });
    }
}
