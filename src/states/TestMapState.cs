using Bitron.Ecs;
using Haldric.Wdk;

public partial class TestMapState : GameState
{
    private string _mapName;

    public TestMapState(EcsWorld world, string mapName) : base(world)
    {
        _mapName = mapName;

        AddInitSystem(new SpawnCameraOperatorSystem(this));

        AddUpdateSystem(new UpdateTerrainInfoSystem());
        AddUpdateSystem(new UpdateStatsInfoSystem());
        AddUpdateSystem(new UpdateHoveredLocationSystem(this));
        AddUpdateSystem(new UpdateMapCursorSystem());
        AddUpdateSystem(new UpdateCameraOperatorSystem());

        AddEventSystem<UpdateMapEvent>(new UpdateMapEventSystem());
        AddEventSystem<UpdateTerrainMeshEvent>(new UpdateTerrainMeshEventSystem());
        AddEventSystem<UpdateTerrainFeaturePopulatorEvent>(new UpdateTerrainFeaturePopulatorEventSystem());
        AddEventSystem<LoadMapEvent>(new LoadMapEventSystem());
        AddEventSystem<DespawnMapEvent>(new DespawnMapEventSystem());
        AddEventSystem<SpawnScheduleEvent>(new SpawnScheduleEventSystem(this));
        AddEventSystem<SpawnMapEvent>(new SpawnMapEventSystem(this));
        AddEventSystem<ChangeDaytimeEvent>(new ChangeDaytimeEventSystem());

        AddDestroySystem(new DespawnCameraOperatorSystem());
    }

    public override void Enter(GameStateController gameStates)
    {
        _world.Spawn().Add(new SpawnScheduleEvent("DefaultSchedule"));
        _world.Spawn().Add(new LoadMapEvent(_mapName));
        _world.Spawn().Add(new ChangeDaytimeEvent());
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.RemoveResource<Schedule>();

        _world.Spawn().Add(new DespawnMapEvent());
    }

    public override void Input(GameStateController gameStates, Godot.InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {
            gameStates.PopState();
        }

        if (e.IsActionPressed("ui_accept"))
        {
            _world.Spawn().Add(new ChangeDaytimeEvent());
        }
    }
}