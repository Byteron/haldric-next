using Godot;
using RelEcs;
using RelEcs.Godot;

public class UpdateStatsInfoSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<DebugPanel>(out var panel)) return;
        if (!commands.TryGetElement<WorldInfo>(out var info)) return;

        var s = $"Entities: {info.EntityCount}\nArchetypes: {info.ArchetypeCount}\nSystems: {info.SystemCount}";
        
        foreach (var (type, time) in info.SystemExecutionTimes)
        {
            s += $"\n{type.Name}: {time}";
        }

        panel.StatsLabel.Text = s;
    }
}