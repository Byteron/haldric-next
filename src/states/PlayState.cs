using Bitron.Ecs;

public partial class PlayState : GameState
{
    public PlayState(EcsWorld world) : base(world)
    {
        AddInitSystem(new SpawnCameraOperatorSystem(this));

        AddInputSystem(new SelectLocationSystem(this));
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
        AddEventSystem<SpawnUnitEvent>(new SpawnUnitEventSystem(this));

        AddDestroySystem(new DespawnCameraOperatorSystem());
    }

    public override void Enter(GameStateController gameStates)
    {   
        var hudView = Scenes.Instance.HUDView.Instantiate<HUDView>();
        AddChild(hudView);

        _world.AddResource(hudView);

        _world.Spawn().Add(new LoadMapEvent("map"));

        _world.Spawn().Add(new SpawnUnitEvent("Soldier", Coords.FromOffset(5, 5)));
        _world.Spawn().Add(new SpawnUnitEvent("Soldier", Coords.FromOffset(6, 5)));
        _world.Spawn().Add(new SpawnUnitEvent("Soldier", Coords.FromOffset(6, 6)));
        _world.Spawn().Add(new SpawnUnitEvent("Soldier", Coords.FromOffset(6, 7)));
    }

    public override void Exit(GameStateController gameStates)
    {
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