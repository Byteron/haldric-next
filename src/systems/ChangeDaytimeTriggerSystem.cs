using RelEcs;
using Haldric.Wdk;

public class ChangeDaytimeTrigger { }

public class ChangeDaytimeTriggerSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<Schedule>(out var schedule)) return;
        commands.Receive((ChangeDaytimeTrigger _) => { schedule.Next(); });
    }
}