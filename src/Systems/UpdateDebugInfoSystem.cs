using System;
using Godot;
using RelEcs;

public class UpdateDebugInfoSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        if (!this.TryGetElement<DebugPanel>(out var panel)) return;
        if (!this.TryGetElement<WorldInfo>(out var info)) return;

        var s = $"FPS: {Engine.GetFramesPerSecond()}\nEntities: {info.EntityCount}\nElements: {info.ElementCount}";
        s += $"\nArchetypes: {info.ArchetypeCount}\nSystems: {info.SystemCount}\nQueries: {info.CachedQueryCount}";
        
        TimeSpan systemTime = default;
        
        foreach (var (type, time) in info.SystemExecutionTimes)
        {
            systemTime += time;
        }
        s += $"\nSystemTime: {systemTime}";

        panel.StatsLabel.Text = s;
    }
}