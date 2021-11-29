using Bitron.Ecs;
using Haldric.Wdk;
using Godot;

public struct ChangeDaytimeEvent { }

public class ChangeDaytimeEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<ChangeDaytimeEvent>().End();

        if (!world.TryGetResource<Schedule>(out var schedule))
        {
            return;
        }

        foreach (var eventId in query)
        {
            schedule.Next();         
        }
    }
}