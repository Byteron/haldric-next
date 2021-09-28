using Bitron.Ecs;

public partial class EditorState : GameState
{
    EcsPackedEntity _editorEntityPacked;

    public EditorState(EcsWorld world) : base(world)
    {
        AddInputSystem(new UpdateMapCursorSystem(this));
        AddInputSystem(new EditorEditSystem(this));
        AddInputSystem(new SelectLocationSystem(this));
        AddInputSystem(new UpdateTerrainInfoSystem());
        AddInputSystem(new LocationHighlightSystem());

        AddUpdateSystem(new CameraOperatorSystem(this));
        AddUpdateSystem(new UpdateStatsInfoSystem(this));

        AddEventSystem<UpdateMapEvent>(new UpdateMapEventSystem());
        AddEventSystem<UpdateTerrainMeshEvent>(new UpdateTerrainMeshEventSystem());
        AddEventSystem<UpdateTerrainFeaturePopulatorEvent>(new UpdateTerrainFeaturePopulatorEventSystem());
        AddEventSystem<SaveMapEvent>(new SaveMapEventSystem());
        AddEventSystem<LoadMapEvent>(new LoadMapEventSystem());
        AddEventSystem<DestroyMapEvent>(new DestroyMapEventSystem());
        AddEventSystem<CreateMapEvent>(new CreateMapEventSystem(this));
    }

    public override void Enter(GameStateController gameStates)
    {

        var editorView = Scenes.Instance.EditorView.Instantiate<EditorView>();
        AddChild(editorView);

        var editorEntity = _world.Spawn().Add(new NodeHandle<EditorView>(editorView)).Entity();
        _editorEntityPacked = _world.PackEntity(editorEntity);

        _world.Spawn().Add(new CreateMapEvent(40, 40));
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.Spawn().Add(new DestroyMapEvent());
        _editorEntityPacked.Unpack(_world, out var editorEntity); 
        _world.DespawnEntity(editorEntity);
    }

    public override void Input(GameStateController gameStates, Godot.InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {
            gameStates.PopState();
        }
    }
}