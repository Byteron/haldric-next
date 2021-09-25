using Leopotam.Ecs;
using Godot;

public partial class GameState : Node3D
{
    protected EcsWorld _world = null;

    private EcsSystems _inputSystems = null;
    private EcsSystems _updateSystems = null;
    private EcsSystems _eventSystems = null;

    public GameState(EcsWorld world)
    {
        _world = world;

        _inputSystems = new EcsSystems(world);
        _updateSystems = new EcsSystems(world);
        _eventSystems = new EcsSystems(world);
    }

    public override void _EnterTree()
    {
        _inputSystems.Init();
        _updateSystems.Init();
        _eventSystems.Init();
    }

    public override void _ExitTree()
    {
        _inputSystems.Destroy();
        _updateSystems.Destroy();
        _eventSystems.Destroy();
    }

    public virtual void Enter(GameStateController gameStates) { }
    public virtual void Exit(GameStateController gameStates) { }
    public virtual void Pause(GameStateController gameStates) { }
    public virtual void Continue(GameStateController gameStates) { }

    public virtual void Update(GameStateController gameStates, float delta) { }
    public virtual void Input(GameStateController gameStates, InputEvent e) { }

    public void RunInputSystems()
    {
        _inputSystems.Run();
    }

    public void RunUpdateSystems()
    {
        _updateSystems.Run();
    }

    public void RunEventSystems()
    {
        _eventSystems.Run();
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