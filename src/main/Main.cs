using Godot;
using System;
using Leopotam.Ecs;

public partial class Main : Node
{
    EcsWorld _world;
    EcsSystems _systems;

    public override void _Ready()
    {
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        _systems.Add(new MapSpawnSystem(this));
        
        _systems.Init();    
    }

    public override void _Process(float delta)
    {
        _systems.Run();
    }
}
