using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using RelEcs;

public class AppFeature : Feature
{
    public override void Init()
    {
        EnableSystems.Add(new InitAppFeatureSystem());
    }
}

public partial class InitAppFeatureSystem : RefCounted, ISystem
{
    struct LoadingData
    {
        public string Info;
        public Action<World> Action;
    }

    Queue<LoadingData> _loadingStates = new();
    
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
        
        // _loadingStates.Enqueue(new LoadingData{ Info = "Units", Action = Data.Instance.LoadUnits});
        // _loadingStates.Enqueue(new LoadingData{ Info = "Schedules", Action = Data.Instance.LoadSchedules});
        // _loadingStates.Enqueue(new LoadingData{ Info = "Factions", Action = Data.Instance.LoadFactions});
        _loadingStates.Enqueue(new LoadingData{ Info = "Terrain", Action = Data.Instance.LoadTerrain});
        _loadingStates.Enqueue(new LoadingData{ Info = "Maps", Action = Data.Instance.LoadMaps});
        
        Loading();
    }

    async void Loading()
    {
        var sceneTree = this.GetElement<SceneTree>();

        while (_loadingStates.Count > 0)
        {
            var loadingData = _loadingStates.Dequeue();
            GD.Print($"loading {loadingData.Info}...");
            await ToSignal(sceneTree, "process_frame");
            loadingData.Action(World);
        }
        GD.Print($"loading finished!");

        this.GetElement<Features>().EnableFeature<MenuFeature>();
    }

    public void CallAction(Action action)
    {
        action();
    }
}