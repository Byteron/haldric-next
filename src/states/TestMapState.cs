using RelEcs;
using Godot;
using Haldric.Wdk;

public partial class TestMapState : GameState
{
    readonly string _mapName;

    public TestMapState(string mapName)
    {
        _mapName = mapName;        
    }

    public override void Init(GameStateController gameStates)
    {
        var initSystem = new TestMapStateInitSystem();
        initSystem.MapName = _mapName;

        InitSystems.Add(initSystem)
            .Add(new SpawnCameraOperatorSystem(this));

        InputSystems.Add(new SelectUnitSystem())
            .Add(new SelectTargetSystem())
            .Add(new DeselectUnitSystem())
            .Add(new TestRecruitInputSystem())
            .Add(new NextUnitInputSystem())
            .Add(new SuspendUnitInputSystem())
            .Add(new ExitFromStateSystem());

        UpdateSystems.Add(new UpdateTerrainInfoSystem())
            .Add(new UpdateUnitPlateSystem())
            .Add(new UpdateStatsInfoSystem())
            .Add(new UpdateHoveredLocationSystem(this))
            .Add(new PreviewPathSystem())
            .Add(new UpdateMapCursorSystem())
            .Add(new UpdateCameraOperatorSystem())
            .Add(new UpdateHoveredUnitSystem())
            .Add(new MoveUnitSystem())
            .Add(new FocusCameraEventSystem())
            .Add(new UpdateMapTriggerSystem())
            .Add(new UpdateTerrainMeshEventSystem())
            .Add(new UpdateTerrainFeaturePopulatorEventSystem())
            .Add(new LoadMapEventSystem())
            .Add(new DespawnMapTriggerSystem())
            .Add(new SpawnScheduleTriggerSystem(this))
            .Add(new SpawnMapTriggerSystem(this))
            .Add(new SpawnPlayerEventSystem())
            .Add(new SpawnUnitTriggerSystem(this))
            .Add(new RecruitUnitTriggerSystem(this))
            .Add(new UnitHoveredEventSystem())
            .Add(new UnitDeselectedEventSystem())
            .Add(new UnitSelectedEventSystem())
            .Add(new MoveUnitTriggerSystem())
            .Add(new HighlightLocationsEventSystem())
            .Add(new TurnEndTriggerSystem())
            .Add(new ChangeDaytimeTriggerSystem())
            .Add(new CaptureVillageTriggerSystem(this));

        ExitSystems.Add(new TestMapStateExitSystem())
            .Add(new DespawnCameraOperatorSystem());
    }  
}

public class TestMapStateExitSystem : ISystem
{
    public void Run(Commands commands)
    {
        var turnPanel = commands.GetElement<TurnPanel>();
        turnPanel.QueueFree();
        commands.RemoveElement<TurnPanel>();

        var terrainPanel = commands.GetElement<TerrainPanel>();
        terrainPanel.QueueFree();
        commands.RemoveElement<TerrainPanel>();

        var unitPanel = commands.GetElement<UnitPanel>();
        unitPanel.QueueFree();
        commands.RemoveElement<UnitPanel>();

        commands.RemoveElement<Commander>();
        commands.RemoveElement<Schedule>();
        commands.RemoveElement<Scenario>();
        commands.RemoveElement<LocalPlayer>();

        commands.Send(new DespawnMapTrigger());
    }
}

public partial class TestMapStateInitSystem : Resource, ISystem
{
    public string MapName;

    Commands _commands;

    public void Run(Commands commands)
    {
        this._commands = commands;

        var localPlayer = new LocalPlayer();
        localPlayer.Id = 0;

        commands.AddElement(localPlayer);
        commands.AddElement(new Commander());
        commands.AddElement(new Scenario());

        var canvas = commands.GetElement<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(1);

        var turnPanel = Scenes.Instantiate<TurnPanel>();
        turnPanel.Connect(nameof(TurnPanel.EndTurnButtonPressed), new Callable(this, nameof(OnTurnEndButtonPressed)));
        canvasLayer.AddChild(turnPanel);
        commands.AddElement(turnPanel);

        var unitPanel = Scenes.Instantiate<UnitPanel>();
        canvasLayer.AddChild(unitPanel);
        commands.AddElement(unitPanel);

        var terrainPanel = Scenes.Instantiate<TerrainPanel>();
        canvasLayer.AddChild(terrainPanel);
        commands.AddElement(terrainPanel);

        commands.Send(new SpawnPlayerEvent(0, 0, Coords.FromOffset(1, 1), "Test", 1000));
        commands.Send(new SpawnPlayerEvent(0, 1, Coords.FromOffset(2, 2), "Test", 1000));

        commands.Send(new SpawnScheduleTrigger("DefaultSchedule"));
        commands.Send(new LoadMapEvent(MapName));
        commands.Send(new TurnEndTrigger());
    }

    public void OnTurnEndButtonPressed()
    {
        _commands.Send(new TurnEndTrigger());
    }
}