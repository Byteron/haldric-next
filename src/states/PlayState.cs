using Bitron.Ecs;

public partial class PlayState : GameState
{
    public PlayState(EcsWorld world) : base(world)
    {
        AddInitSystem(new SpawnCameraOperatorSystem(this));

        AddInputSystem(new SelectUnitSystem());
        AddInputSystem(new SelectTargetSystem());
        AddInputSystem(new DeselectUnitSystem());
        AddInputSystem(new UndoCommandSystem());

        AddUpdateSystem(new UpdateTerrainInfoSystem());
        AddUpdateSystem(new UpdateHoveredLocationSystem(this));
        AddUpdateSystem(new UpdateMapCursorSystem());
        AddUpdateSystem(new UpdateCameraOperatorSystem());
        AddUpdateSystem(new UpdateHoveredUnitSystem());
        AddUpdateSystem(new MoveUnitSystem());
        AddUpdateSystem(new UpdateStatsInfoSystem());
        AddUpdateSystem(new UpdateUnitPlateSystem());

        AddEventSystem<UpdateMapEvent>(new UpdateMapEventSystem());
        AddEventSystem<UpdateTerrainMeshEvent>(new UpdateTerrainMeshEventSystem());
        AddEventSystem<UpdateTerrainFeaturePopulatorEvent>(new UpdateTerrainFeaturePopulatorEventSystem());
        AddEventSystem<LoadMapEvent>(new LoadMapEventSystem());
        AddEventSystem<DespawnMapEvent>(new DespawnMapEventSystem());
        AddEventSystem<SpawnMapEvent>(new SpawnMapEventSystem(this));
        AddEventSystem<SpawnUnitsEvent>(new SpawnUnitsEventSystem());
        AddEventSystem<SpawnUnitEvent>(new SpawnUnitEventSystem(this));
        AddEventSystem<UnitHoveredEvent>(new UnitHoveredEventSystem());
        AddEventSystem<UnitSelectedEvent>(new UnitSelectedEventSystem());
        AddEventSystem<UnitDeselectedEvent>(new UnitDeselectedEventSystem());
        AddEventSystem<HighlightLocationEvent>(new HighlightLocationsEventSystem());
        AddEventSystem<DamageEvent>(new DamageEventSystem());
        AddEventSystem<MissEvent>(new MissEventSystem());
        AddEventSystem<GainExperienceEvent>(new GainExperienceEventSystem());
        AddEventSystem<AdvanceEvent>(new AdvanceEventSystem());
        AddEventSystem<DeathEvent>(new DeathEventSystem());
        AddEventSystem<CaptureVillageEvent>(new CaptureVillageEventSystem(this));
        AddEventSystem<SpawnFloatingLabelEvent>(new SpawnFloatingLabelEventSystem());
        AddEventSystem<TurnEndEvent>(new TurnEndEventSystem());

        AddDestroySystem(new DespawnCameraOperatorSystem());
    }

    public override void Enter(GameStateController gameStates)
    {
        var scenario = new Scenario();

        _world.AddResource(scenario);
        _world.AddResource(new Commander());

        var hudView = Scenes.Instance.HUDView.Instantiate<HUDView>();
        AddChild(hudView);

        _world.AddResource(hudView);

        _world.Spawn().Add(new LoadMapEvent("map"));
        _world.Spawn().Add(new SpawnUnitsEvent());
        _world.Spawn().Add(new TurnEndEvent());
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.RemoveResource<Scenario>();
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