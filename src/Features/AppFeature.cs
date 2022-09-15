using Godot;
using RelEcs;

public class AppFeature : Feature
{
    public override void Init()
    {
        EnableSystems.Add(new EnableAppFeatureSystem());
    }
}

public class EnableAppFeatureSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
	    var tree = this.GetElement<SceneTree>();
        var canvas = new Canvas();
		canvas.Name = "Canvas";
	    tree.CurrentScene.AddChild(canvas);
		this.AddElement(canvas);

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

        var features = this.GetElement<Features>();
        features.EnableFeature<MenuFeature>();
    }
}