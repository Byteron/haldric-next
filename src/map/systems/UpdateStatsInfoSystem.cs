using Godot;
using Leopotam.Ecs;

public class UpdateStatsInfoSystem : IEcsRunSystem
{
    EcsWorld _world;
    Node3D _parent;

    public UpdateStatsInfoSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run()
    {
        var s = "FPS: " + Engine.GetFramesPerSecond();
        s += "\nEntities: " + _world.GetStats().ActiveEntities;
        s += "\nComponents: " + _world.GetStats().Components;
        
        _parent.GetTree().CallGroup("StatsLabel", "set", "text", s);
    }
}