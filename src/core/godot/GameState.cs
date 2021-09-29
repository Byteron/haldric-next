using Godot;
using Bitron.Ecs;

public partial class GameState : Node3D
{
    protected EcsWorld _world = null;

    private EcsSystemGroup _initSystems = null;
    private EcsSystemGroup _inputSystems = null;
    private EcsSystemGroup _updateSystems = null;
    private EcsSystemGroup _eventSystems = null;
    private EcsSystemGroup _destroySystems = null;

    public GameState(EcsWorld world)
    {
        _world = world;

        _initSystems = new EcsSystemGroup();
        _inputSystems = new EcsSystemGroup();
        _updateSystems = new EcsSystemGroup();
        _eventSystems = new EcsSystemGroup();
        _destroySystems = new EcsSystemGroup();
    }

    public override void _EnterTree()
    {
        _initSystems.Run(_world);
    }

    public override void _ExitTree()
    {
        _destroySystems.Run(_world);
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

    protected void AddInitSystem(IEcsSystem system)
    {
        _initSystems.Add(system);
    }

    protected void AddDestroySystem(IEcsSystem system)
    {
        _destroySystems.Add(system);
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