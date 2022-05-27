using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;
using Godot;

public class SpawnScheduleTrigger
{
    public readonly string Id;
    public readonly int Index;

    public SpawnScheduleTrigger(string id, int index = 0)
    {
        Id = id;
        Index = index;
    }
}

public class SpawnScheduleTriggerSystem : ISystem
{
    readonly Node3D _parent;

    public SpawnScheduleTriggerSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(Commands commands)
    {
        commands.Receive((SpawnScheduleTrigger spawnEvent) =>
        {
            var schedule = Data.Instance.Schedules[spawnEvent.Id].Instantiate<Schedule>();
            _parent.AddChild(schedule);

            schedule.Set(spawnEvent.Index);

            commands.AddElement(schedule);
        });
    }
}
