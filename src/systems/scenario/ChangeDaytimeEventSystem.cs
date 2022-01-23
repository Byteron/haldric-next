using Bitron.Ecs;
using Haldric.Wdk;
using Godot;

public struct ChangeDaytimeEvent { }

public class ChangeDaytimeEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {

        if (!world.TryGetResource<Schedule>(out var schedule))
        {
            return;
        }

        world.ForEach((ref ChangeDaytimeEvent e) => { schedule.Next(); });
    }
}