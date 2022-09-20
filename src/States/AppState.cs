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
        public World World { get; set; }

        public void Run()
        {
            var tree = this.GetTree();

            var canvas = new Canvas();
            canvas.Name = "Canvas";
            this.AddElement(canvas);
            tree.CurrentScene.AddChild(canvas);

            this.AddElement(new ServerConfig
            {
                Host = "49.12.208.4",
                Port = 7350,
                Scheme = "http",
                ServerKey = "defaultkey",
            });

            this.AddElement(new LobbyConfig
            {
                RoomName = "general",
                Persistence = true,
                Hidden = false,
            });
            
            this.AddElement(new Commander());
            
            this.LoadTerrains();
            this.LoadTerrainGraphics();
            this.LoadScenarios();
            this.LoadUnits();
            
            var layer = canvas.GetCanvasLayer(3);
            var debugPanel = Scenes.Instantiate<DebugPanel>();
            layer.AddChild(debugPanel);
            this.AddElement(debugPanel);

            this.PushState(new MenuState());
        }
    }
}
