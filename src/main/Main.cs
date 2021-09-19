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
			.Add(new SpawnMapSystem(this))
			.Add(new MoveUnitSystem())
			.Add(new CreateUnitEventSystem(this))
			.Add(new UpdateMapEventSystem())
			.Add(new UpdateTerrainMeshEventSystem())
			.Add(new UpdateTerrainFeaturePopulatorEventSystem())
			.Add(new SaveMapEventSystem())
			.Add(new LoadMapEventSystem())
			.Add(new DestroyMapEventSystem())
			.Add(new CreateMapEventSystem(this))
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
