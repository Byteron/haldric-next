using Bitron.Ecs;

public partial class PlayState : GameState
{
    public PlayState(EcsWorld world) : base(world)
    {
        AddInitSystem(new SpawnCameraOperatorSystem(this));
        AddInputSystem(new SelectUnitSystem(this));
        AddInputSystem(new DeselectUnitSystem(this));
        AddInputSystem(new UndoCommandSystem());
        AddInputSystem(new UpdateTerrainInfoSystem());

        AddUpdateSystem(new LocationHighlightSystem());
        AddUpdateSystem(new UpdateMapCursorSystem(this));
        AddUpdateSystem(new UpdateCameraOperatorSystem());
        AddUpdateSystem(new MoveUnitSystem());
        AddUpdateSystem(new UpdateStatsInfoSystem());

        AddEventSystem<UpdateMapEvent>(new UpdateMapEventSystem());
        AddEventSystem<UpdateTerrainMeshEvent>(new UpdateTerrainMeshEventSystem());
        AddEventSystem<UpdateTerrainFeaturePopulatorEvent>(new UpdateTerrainFeaturePopulatorEventSystem());
        AddEventSystem<LoadMapEvent>(new LoadMapEventSystem());
        AddEventSystem<DespawnMapEvent>(new DespawnMapEventSystem());
        AddEventSystem<SpawnMapEvent>(new SpawnMapEventSystem(this));
        AddEventSystem<SpawnUnitsEvent>(new SpawnUnitsEventSystem());
        AddEventSystem<SpawnUnitEvent>(new SpawnUnitEventSystem(this));
        AddEventSystem<UnitSelectedEvent>(new UnitSelectedEventSystem(this));
        AddEventSystem<HighlightLocationEvent>(new HighlightLocationsEventSystem());

        AddDestroySystem(new DespawnCameraOperatorSystem());
    }

    public override void Enter(GameStateController gameStates)
    {   
        _world.AddResource(new Commander());

        var hudView = Scenes.Instance.HUDView.Instantiate<HUDView>();
        AddChild(hudView);

        _world.AddResource(hudView);

        _world.Spawn().Add(new LoadMapEvent("map"));
        _world.Spawn().Add(new SpawnUnitsEvent());
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.RemoveResource<Commander>();
        
        _world.RemoveResource<HUDView>();
        _world.Spawn().Add(new DespawnMapEvent());
    }

    public override void Input(GameStateController gameStates, Godot.InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {
            gameStates.PopState();
        }
    }
}