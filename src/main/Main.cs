using Godot;
using Leopotam.Ecs;

public partial class Main : Node3D
{
    public static Main Instance { get; private set; }

    EcsWorld _world;
    EcsSystems _inputSystems;
    EcsSystems _processSystems;

    public EcsWorld World { get { return _world; } }

    public override void _Ready()
    {
        Instance = this;

        _world = new EcsWorld();

        Data.Instance.World = _world;
        Data.Instance.Scan();

        _inputSystems = new EcsSystems(_world);
        _processSystems = new EcsSystems(_world);

        _inputSystems
            .Add(new EditorEditSystem(this))
            .Add(new CollisionDetectorSystem(this))
            .Add(new LocationHighlightSystem(this));

        _processSystems
            .Add(new SpawnMapSystem(this))
            .Add(new UpdateMapEventSystem())
            .Add(new UpdateTerrainMeshEventSystem())
            .Add(new UpdateTerrainFeaturePopulatorEventSystem())
            .Add(new SaveMapEventSystem())
            .Add(new LoadMapEventSystem())
            .Add(new DestroyMapEventSystem())
            .Add(new CreateMapEventSystem(this));

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
