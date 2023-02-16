using Godot;
using RelEcs;

public class AppState : IState
{
    public void Enable(World world)
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
        
        LoadTerrains(world);
        LoadTerrainGraphics(world);
        LoadScenarios(world);
        LoadUnits(world);
            
        var layer = canvas.GetCanvasLayer(3);
        var debugPanel = Scenes.Instantiate<DebugPanel>();
        layer.AddChild(debugPanel);
        world.AddElement(debugPanel);
        
        world.EnableState<MenuState>();
    }

    public void Update(World world)
    {
        UpdateDebugInfo(world);
    }

    public void Disable(World world)
    {
        world.GetTree().Quit();
    }
}
