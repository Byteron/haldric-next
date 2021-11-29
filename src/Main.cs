using Godot;
using Bitron.Ecs;

public partial class Main : Node3D
{
    public static Main Instance { get; private set; }
    
    public EcsWorld World { get; private set; } = new EcsWorld();

    public override void _Ready()
    {
        Instance = this;

        Inititalize();
    }

    public void Inititalize()
    {
        World.AddResource(GetTree());

        var gameStateController = new GameStateController();
        gameStateController.Name = "GameStateController";
        AddChild(gameStateController);
        World.AddResource(gameStateController);

        var canvas = new Canvas();
        canvas.Name = "Canvas";
        AddChild(canvas);
        World.AddResource(canvas);

        World.AddResource(new ServerSettings
        {
            Host = "49.12.208.4",
            Port = 7350,
            Scheme = "http",
            ServerKey = "defaultkey",
        });

        World.AddResource(new LobbySettings
        {
            RoomName = "general",
            Persistence = true,
            Hidden = false,
        });

        gameStateController.PushState(new ApplicationState(World));
    }

    public override void _ExitTree()
    {
        World.Destroy();
    }
}
