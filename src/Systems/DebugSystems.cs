using System;
using Godot;
using RelEcs;

public static class DebugSystems
{
    public static void UpdateDebugInfo(this World world)
    {
        if (!world.TryGetElement<DebugPanel>(out var panel)) return;
        if (!world.TryGetElement<WorldInfo>(out var info)) return;

        var s = $"FPS: {Engine.GetFramesPerSecond()}\nEntities: {info.EntityCount}\nElements: {info.ElementCount}";
        s += $"\nArchetypes: {info.ArchetypeCount}\nSystems: {info.SystemCount}\nQueries: {info.CachedQueryCount}";

        TimeSpan systemTime = default;

        foreach (var (_, time) in info.SystemExecutionTimes)
        {
            systemTime += time;
        }

        s += $"\nSystemTime: {systemTime}";

        panel.StatsLabel.Text = s;
    }
}