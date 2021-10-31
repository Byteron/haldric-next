using Bitron.Ecs;

public partial class EditorState : GameState
{
    public EditorState(EcsWorld world) : base(world)
    {
        AddInitSystem(new SpawnCameraOperatorSystem(this));

        AddInputSystem(new EditorEditTerrainSystem(this));
        AddInputSystem(new EditorEditPlayerSystem(this));
        AddInputSystem(new UndoCommandSystem());

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

        AddDestroySystem(new DespawnCameraOperatorSystem());
    }

    public override void Enter(GameStateController gameStates)
    {
        _world.AddResource(new Commander());

        var editorView = Scenes.Instance.EditorView.Instantiate<EditorView>();
        AddChild(editorView);

        _world.AddResource(editorView);

        _world.Spawn().Add(new SpawnMapEvent(40, 40));
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.RemoveResource<Commander>();
        _world.RemoveResource<EditorView>();
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