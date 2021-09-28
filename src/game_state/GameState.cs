using Godot;
using Bitron.Ecs;

public partial class GameState : Node3D
{
    protected EcsWorld _world = null;

    private EcsSystemGroup _inputSystems = null;
    private EcsSystemGroup _updateSystems = null;
    private EcsSystemGroup _eventSystems = null;

    public GameState(EcsWorld world)
    {
        _world = world;

        _inputSystems = new EcsSystemGroup();
        _updateSystems = new EcsSystemGroup();
        _eventSystems = new EcsSystemGroup();
    }

    public override void _EnterTree()
    {
        _inputSystems.Init(_world);
        _updateSystems.Init(_world);
        _eventSystems.Init(_world);
    }

    public override void _ExitTree()
    {
        _inputSystems.Destroy(_world);
        _updateSystems.Destroy(_world);
        _eventSystems.Destroy(_world);
    }

    public virtual void Enter(GameStateController gameStates) { }
    public virtual void Exit(GameStateController gameStates) { }
    public virtual void Pause(GameStateController gameStates) { }
    public virtual void Continue(GameStateController gameStates) { }

    public virtual void Update(GameStateController gameStates, float delta) { }
    public virtual void Input(GameStateController gameStates, InputEvent e) { }

    public void RunInputSystems()
    {
        _inputSystems.Run(_world);
    }

    public void RunUpdateSystems()
    {
        _updateSystems.Run(_world);
    }

    public void RunEventSystems()
    {
        _eventSystems.Run(_world);
    }

    protected void AddInputSystem(IEcsSystem system)
    {
        _inputSystems.Add(system);
    }

    protected void AddUpdateSystem(IEcsSystem system)
    {
        _updateSystems.Add(system);
    }

    protected void AddEventSystem<T>(IEcsSystem system) where T : struct
    {
        _eventSystems.Add(system).OneFrame<T>();
    }
}