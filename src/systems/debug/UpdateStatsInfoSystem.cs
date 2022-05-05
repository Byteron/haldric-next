using Godot;
using RelEcs;
using RelEcs.Godot;

public class UpdateStatsInfoSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<WorldInfo>(out var worldInfo))
        {
            return;
        }

        var s = "FPS: " + Engine.GetFramesPerSecond();
        s += "\nEntities: " + worldInfo.EntityCount;
        s += "\nComponents: " + worldInfo.ComponentCount;

        commands.GetElement<DebugPanel>().StatsLabel.Text = s;
    }
}