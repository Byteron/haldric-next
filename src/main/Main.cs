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

        Data.Instance.Scan();

        _inputSystems = new EcsSystems(_world);
        _processSystems = new EcsSystems(_world);

        _inputSystems
            .Add(new EditorEditSystem(this))
            .Add(new UpdateMapCursorSystem(this))
            .Add(new SelectLocationSystem(this))
            .Add(new CommanderUndoSystem())
            .Add(new UpdateTerrainInfoSystem())
            .Add(new LocationHighlightSystem());

        _processSystems
            .Add(new MoveUnitSystem())
            .Add(new CreateUnitEventSystem(this)).OneFrame<CreateUnitEvent>()
            .Add(new UpdateMapEventSystem()).OneFrame<UpdateMapEvent>()
            .Add(new UpdateTerrainMeshEventSystem()).OneFrame<UpdateTerrainMeshEvent>()
            .Add(new UpdateTerrainFeaturePopulatorEventSystem()).OneFrame<UpdateTerrainFeaturePopulatorEvent>()
            .Add(new SaveMapEventSystem()).OneFrame<SaveMapEvent>()
            .Add(new LoadMapEventSystem()).OneFrame<LoadMapEvent>()
            .Add(new DestroyMapEventSystem()).OneFrame<DestroyMapEvent>()
            .Add(new CreateMapEventSystem(this)).OneFrame<CreateMapEvent>()
            .Add(new MoveUnitCommandSystem())
            .Add(new UpdateStatsInfoSystem(this));

        var commanderEntity = _world.NewEntity();
        commanderEntity.Get<Commander>();

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
