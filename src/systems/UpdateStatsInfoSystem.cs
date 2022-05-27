using RelEcs;

public class UpdateStatsInfoSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<DebugPanel>(out var panel)) return;
        if (!commands.TryGetElement<WorldInfo>(out var info)) return;

        var s = $"Entities: {info.EntityCount}\nElements: {info.ElementCount}\nArchetypes: {info.ArchetypeCount}";
        s += $"\nSystems: {info.SystemCount}\nQueries: {info.CachedQueryCount}";
        
        foreach (var (type, time) in info.SystemExecutionTimes)
        {
            s += $"\n{type.Name}: {time}";
        }

        panel.StatsLabel.Text = s;
    }
}