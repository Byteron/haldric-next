using Godot;
using RelEcs;

public partial class AppState : GameState
{
    public override void Init()
    {
        Enter.Add(new EnterSystem());
    }

    partial class EnterSystem : RefCounted, ISystem
    {
        public void Run(World world)
        {
            var tree = world.GetTree();

            var canvas = new Canvas();
            canvas.Name = "Canvas";
            world.AddElement(canvas);
            tree.CurrentScene.AddChild(canvas);

            world.AddElement(new ServerConfig
            {
                Host = "49.12.208.4",
                Port = 7350,
                Scheme = "http",
                ServerKey = "defaultkey",
            });

            world.AddElement(new LobbyConfig
            {
                RoomName = "general",
                Persistence = true,
                Hidden = false,
            });
            
            world.AddElement(new Commander());
            
            world.LoadTerrains();
            world.LoadTerrainGraphics();
            world.LoadScenarios();
            world.LoadUnits();
            
            var layer = canvas.GetCanvasLayer(3);
            var debugPanel = Scenes.Instantiate<DebugPanel>();
            layer.AddChild(debugPanel);
            world.AddElement(debugPanel);

            world.PushState(new MenuState());
        }
    }
}
