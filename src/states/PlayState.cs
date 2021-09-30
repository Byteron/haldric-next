using Bitron.Ecs;

public partial class PlayState : GameState
{
    public PlayState(EcsWorld world) : base(world)
    {
        AddInitSystem(new SpawnCameraOperatorSystem(this));

        AddInputSystem(new UpdateMapCursorSystem(this));
        AddInputSystem(new SelectLocationSystem(this));
        AddInputSystem(new CommanderUndoSystem());
        AddInputSystem(new UpdateTerrainInfoSystem());
        AddInputSystem(new LocationHighlightSystem());

        AddUpdateSystem(new UpdateCameraOperatorSystem());
        AddUpdateSystem(new MoveUnitSystem());
        AddUpdateSystem(new UpdateStatsInfoSystem());

        AddEventSystem<UpdateMapEvent>(new UpdateMapEventSystem());
        AddEventSystem<UpdateTerrainMeshEvent>(new UpdateTerrainMeshEventSystem());
        AddEventSystem<UpdateTerrainFeaturePopulatorEvent>(new UpdateTerrainFeaturePopulatorEventSystem());
        AddEventSystem<LoadMapEvent>(new LoadMapEventSystem());
        AddEventSystem<DestroyMapEvent>(new DestroyMapEventSystem());
        AddEventSystem<CreateMapEvent>(new CreateMapEventSystem(this));
        AddEventSystem<CreateUnitEvent>(new CreateUnitEventSystem(this));

        AddDestroySystem(new DestroyCameraOperatorSystem());
    }

    public override void Enter(GameStateController gameStates)
    {   
        var hudView = Scenes.Instance.HUDView.Instantiate<HUDView>();
        AddChild(hudView);

        _world.AddResource(hudView);

        _world.Spawn().Add(new LoadMapEvent("map"));

        _world.Spawn().Add(new CreateUnitEvent("Soldier", Coords.FromOffset(5, 5)));
        _world.Spawn().Add(new CreateUnitEvent("Soldier", Coords.FromOffset(6, 5)));
        _world.Spawn().Add(new CreateUnitEvent("Soldier", Coords.FromOffset(6, 6)));
        _world.Spawn().Add(new CreateUnitEvent("Soldier", Coords.FromOffset(6, 7)));
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.RemoveResource<HUDView>();
        _world.Spawn().Add(new DestroyMapEvent());
    }

    public override void Input(GameStateController gameStates, Godot.InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {
            gameStates.PopState();
        }
    }
}