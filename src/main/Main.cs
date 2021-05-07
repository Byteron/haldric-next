using Godot;
using System;
using Leopotam.Ecs;

public partial class Main : Node3D
{
    EcsWorld _world;
    EcsSystems _processSystems;
    EcsSystems _inputSystems;

    public override void _Ready()
    {
        _world = new EcsWorld();

        _inputSystems = new EcsSystems(_world);
        _processSystems = new EcsSystems(_world);

        _inputSystems
            .Add(new CollisionDetectorSystem(this));
        
        _processSystems
            .Add(new MapSpawnSystem(this))
            .Add(new UpdateMapEventSystem())
            .Add(new UpdateTerrainMeshEventSystem())
            .Add(new UpdateTerrainFeaturePopulatorEventSystem());

        _inputSystems.Init();
        _processSystems.Init();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _inputSystems.Run();
    }

    public override void _Process(float delta)
    {
        _processSystems.Run();
    }
}
