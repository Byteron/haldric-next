using Godot;
using RelEcs;

public class AppFeature : Feature
{
    public override void Init()
    {
        EnableSystems.Add(new InitAppFeatureSystem());

        UpdateSystems
            .Add(new LoadTerrainDataSystem())
            .Add(new LoadTerrainGraphicDataSystem())
            .Add(new LoadUnitDataSystem())
            .Add(new LoadScenarioDataSystem());
    }
}

public partial class InitAppFeatureSystem : RefCounted, ISystem
{
    public World World { get; set; }

    public void Run()
    {
        var tree = this.GetElement<SceneTree>();

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
        
        this.Send(new LoadTerrainDataTrigger());
        this.Send(new LoadTerrainGraphicDataTrigger());
        // this.Send(new LoadScenarioDataTrigger());
        // this.Send(new LoadUnitDataTrigger());

        this.GetElement<Features>().EnableFeature<MenuFeature>();
    }
}