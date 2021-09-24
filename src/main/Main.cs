using Godot;
using Leopotam.Ecs;

public partial class Main : Node3D
{
    public static Main Instance { get; private set; }

    public GameStateController _gameController;

    EcsWorld _world;

    public EcsWorld World { get { return _world; } }
    public GameStateController GameStateController { get { return _gameController; } }

    public override void _Ready()
    {
        Instance = this;

        _world = new EcsWorld();

        Data.Instance.Scan();
        
        var commanderEntity = _world.NewEntity();
        commanderEntity.Get<Commander>();

        _gameController = GetNode<GameStateController>("GameStateController");
        _gameController.PushState(new ApplicationState(_world));
    }
}
