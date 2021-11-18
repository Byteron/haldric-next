using Bitron.Ecs;
using Haldric.Wdk;

public partial class EditorState : GameState
{
    public EditorState(EcsWorld world) : base(world)
    {
        AddInitSystem(new SpawnCameraOperatorSystem(this));

        AddInputSystem(new EditorEditTerrainSystem(this));
        AddInputSystem(new EditorEditPlayerSystem(this));

        AddUpdateSystem(new UpdateTerrainInfoSystem());
        AddUpdateSystem(new UpdateHoveredLocationSystem(this));
        AddUpdateSystem(new UpdateMapCursorSystem());
        AddUpdateSystem(new UpdateCameraOperatorSystem());
        AddUpdateSystem(new UpdateStatsInfoSystem());

        AddEventSystem<UpdateMapEvent>(new UpdateMapEventSystem());
        AddEventSystem<UpdateTerrainMeshEvent>(new UpdateTerrainMeshEventSystem());
        AddEventSystem<UpdateTerrainFeaturePopulatorEvent>(new UpdateTerrainFeaturePopulatorEventSystem());
        AddEventSystem<SaveMapEvent>(new SaveMapEventSystem());
        AddEventSystem<LoadMapEvent>(new LoadMapEventSystem());
        AddEventSystem<DespawnMapEvent>(new DespawnMapEventSystem());
        AddEventSystem<SpawnMapEvent>(new SpawnMapEventSystem(this));
        AddEventSystem<ChangeDaytimeEvent>(new ChangeDaytimeEventSystem());

        AddDestroySystem(new DespawnCameraOperatorSystem());
    }

    public override void Enter(GameStateController gameStates)
    {
        _world.AddResource(new Commander());

        var editorView = Scenes.Instance.EditorView.Instantiate<EditorView>();
        AddChild(editorView);

        _world.AddResource(editorView);

        _world.AddResource(Data.Instance.Schedules["DefaultSchedule"].Instantiate<Schedule>());

        _world.Spawn().Add(new SpawnMapEvent(40, 40));
        _world.Spawn().Add(new ChangeDaytimeEvent());
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.RemoveResource<Commander>();
        _world.RemoveResource<EditorView>();
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