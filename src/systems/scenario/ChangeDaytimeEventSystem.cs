using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;
using Godot;

public struct ChangeDaytimeEvent { }

public class ChangeDaytimeEventSystem : ISystem
{
    public void Run(Commands commands)
    {

        if (!commands.TryGetElement<Schedule>(out var schedule))
        {
            return;
        }

        commands.Receive((ChangeDaytimeEvent e) => { schedule.Next(); });
    }
}