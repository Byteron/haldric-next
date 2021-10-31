using Godot;
using Bitron.Ecs;

public partial class Main : Node3D
{
	public static Main Instance { get; private set; }

	private GameStateController _gameController = new GameStateController();

    public WorldEnvironment Environment { get; private set; }
    public DirectionalLight3D Light { get; private set; }
    
    public EcsWorld World { get; private set; } = new EcsWorld();

	public override void _Ready()
	{
		Instance = this;
		
		_gameController.Name = "GameStateController";
		AddChild(_gameController);

		World.AddResource(_gameController);
		World.AddResource(GetTree());

        Environment = GetNode<WorldEnvironment>("WorldEnvironment");
        Light = GetNode<DirectionalLight3D>("LightContainer/DirectionalLight3D");

        _gameController.PushState(new ApplicationState(World));
    }

	public override void _ExitTree()
	{
		World.Destroy();
	}
}
