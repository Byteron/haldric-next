using Godot;
using Bitron.Ecs;

public partial class Main : Node3D
{
    public static Main Instance { get; private set; }

    public GameStateController _gameController;

    EcsWorld _world;

    public EcsWorld World { get { return _world; } }

    public override void _Ready()
    {
        Instance = this;

        _world = new EcsWorld();

        Data.Instance.Scan();
        
        _gameController = new GameStateController();
        _gameController.Name = "GameStateController";
        AddChild(_gameController);

        _world.AddResource(_gameController);
        
        _world.AddResource(new Commander());

        _gameController.PushState(new ApplicationState(_world));
    }
}
