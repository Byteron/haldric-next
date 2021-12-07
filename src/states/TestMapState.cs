using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public partial class TestMapState : GameState
{
    private string _mapName;

    public TestMapState(EcsWorld world, string mapName) : base(world)
    {
        _mapName = mapName;

        AddInitSystem(new SpawnCameraOperatorSystem(this));

        AddInputSystem(new SelectUnitSystem());
        AddInputSystem(new SelectTargetSystem());
        AddInputSystem(new DeselectUnitSystem());
        AddInputSystem(new TestRecruitInputSystem());
        AddInputSystem(new NextUnitInputSystem());
        AddInputSystem(new SuspendUnitInputSystem());

        AddUpdateSystem(new UpdateTerrainInfoSystem());
        AddUpdateSystem(new UpdateUnitPlateSystem());
        AddUpdateSystem(new UpdateStatsInfoSystem());
        AddUpdateSystem(new UpdateHoveredLocationSystem(this));
        AddUpdateSystem(new PreviewPathSystem());
        AddUpdateSystem(new UpdateMapCursorSystem());
        AddUpdateSystem(new UpdateCameraOperatorSystem());
        AddUpdateSystem(new UpdateHoveredUnitSystem());
        AddUpdateSystem(new MoveUnitSystem());

        AddEventSystem<FocusCameraEvent>(new FocusCameraEventSystem());
        AddEventSystem<UpdateMapEvent>(new UpdateMapEventSystem());
        AddEventSystem<UpdateTerrainMeshEvent>(new UpdateTerrainMeshEventSystem());
        AddEventSystem<UpdateTerrainFeaturePopulatorEvent>(new UpdateTerrainFeaturePopulatorEventSystem());
        AddEventSystem<LoadMapEvent>(new LoadMapEventSystem());
        AddEventSystem<DespawnMapEvent>(new DespawnMapEventSystem());
        AddEventSystem<SpawnScheduleEvent>(new SpawnScheduleEventSystem(this));
        AddEventSystem<SpawnMapEvent>(new SpawnMapEventSystem(this));
        AddEventSystem<SpawnPlayerEvent>(new SpawnPlayerEventSystem());
        AddEventSystem<SpawnUnitEvent>(new SpawnUnitEventSystem(this));
        AddEventSystem<RecruitUnitEvent>(new RecruitUnitEventSystem(this));
        AddEventSystem<UnitHoveredEvent>(new UnitHoveredEventSystem());
        AddEventSystem<UnitDeselectedEvent>(new UnitDeselectedEventSystem());
        AddEventSystem<UnitSelectedEvent>(new UnitSelectedEventSystem());
        AddEventSystem<MoveUnitEvent>(new MoveUnitEventSystem());
        AddEventSystem<HighlightLocationEvent>(new HighlightLocationsEventSystem());
        AddEventSystem<TurnEndEvent>(new TurnEndEventSystem());

        AddEventSystem<ChangeDaytimeEvent>(new ChangeDaytimeEventSystem());
        AddEventSystem<CaptureVillageEvent>(new CaptureVillageEventSystem(this));

        AddDestroySystem(new DespawnCameraOperatorSystem());
    }

    public override void Enter(GameStateController gameStates)
    {
        var localPlayer = new LocalPlayer();
        localPlayer.Id = 0;

        _world.AddResource(localPlayer);
        _world.AddResource(new Commander());
        _world.AddResource(new Scenario());

        var canvas = _world.GetResource<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(1);

        var turnPanel = Scenes.Instance.TurnPanel.Instantiate<TurnPanel>();
        turnPanel.Connect(nameof(TurnPanel.EndTurnButtonPressed), new Callable(this, nameof(OnTurnEndButtonPressed)));
        canvasLayer.AddChild(turnPanel);
        _world.AddResource(turnPanel);
        
        var unitPanel = Scenes.Instance.UnitPanel.Instantiate<UnitPanel>();
        canvasLayer.AddChild(unitPanel);
        _world.AddResource(unitPanel);

        var terrainPanel = Scenes.Instance.TerrainPanel.Instantiate<TerrainPanel>();
        canvasLayer.AddChild(terrainPanel);
        _world.AddResource(terrainPanel);

        _world.Spawn().Add(new SpawnPlayerEvent(0, 0, Coords.FromOffset(1, 1), "Test", 1000));
        _world.Spawn().Add(new SpawnPlayerEvent(0, 1, Coords.FromOffset(2, 2), "Test", 1000));
        
        _world.Spawn().Add(new SpawnScheduleEvent("DefaultSchedule"));
        _world.Spawn().Add(new LoadMapEvent(_mapName));
        _world.Spawn().Add(new TurnEndEvent());
    }

    public override void Exit(GameStateController gameStates)
    {
        var turnPanel = _world.GetResource<TurnPanel>();
        turnPanel.QueueFree();
        _world.RemoveResource<TurnPanel>();

        var terrainPanel = _world.GetResource<TerrainPanel>();
        terrainPanel.QueueFree();
        _world.RemoveResource<TerrainPanel>();

        var unitPanel = _world.GetResource<UnitPanel>();
        unitPanel.QueueFree();
        _world.RemoveResource<UnitPanel>();

        _world.RemoveResource<Commander>();
        _world.RemoveResource<Schedule>();
        _world.RemoveResource<Scenario>();
        _world.RemoveResource<LocalPlayer>();

        _world.Spawn().Add(new DespawnMapEvent());
    }

    public override void Input(GameStateController gameStates, Godot.InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {
            gameStates.PopState();
        }
    }

    public void OnTurnEndButtonPressed()
    {
        _world.Spawn().Add(new TurnEndEvent());
    }
}