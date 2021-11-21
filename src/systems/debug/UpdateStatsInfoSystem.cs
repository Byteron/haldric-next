using Godot;
using Bitron.Ecs;

public class UpdateStatsInfoSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var s = "FPS: " + Engine.GetFramesPerSecond();
        s += "\nEntities: " + world.GetEntitiesCount();
        s += "\nComponents: " + world.GetComponentsCount();

        world.GetResource<DebugPanel>().StatsLabel.Text = s;
    }
}