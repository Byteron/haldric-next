using Godot;
using Bitron.Ecs;

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
        
        _world.AddResource(new Commander());
        ref var commander = ref _world.GetResource<Commander>();

        _gameController = new GameStateController();
        _gameController.Name = "GameStateController";
        AddChild(_gameController);

        _gameController.PushState(new ApplicationState(_world));
    }
}
