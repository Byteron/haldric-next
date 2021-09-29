using Godot;
using Bitron.Ecs;

public class UpdateStatsInfoSystem : IEcsSystem
{
    Node3D _parent;

    public UpdateStatsInfoSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        var s = "FPS: " + Engine.GetFramesPerSecond();
        s += "\nEntities: " + world.GetEntitiesCount();
        s += "\nComponents: " + world.GetComponentsCount();
        
        _parent.GetTree().CallGroup("StatsLabel", "set", "text", s);
    }
}