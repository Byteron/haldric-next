using RelEcs;
using Godot;

public class MenuFeature : Feature
{
    public override void Init()
    {
        EnableSystems.Add(new EnableMenuFeatureSystem());
        DisableSystems.Add(new DisableMenuFeatureSystem());
    }
}

public class EnableMenuFeatureSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        var canvas = this.GetElement<Canvas>();
        var layer = canvas.GetCanvasLayer(1);
        
        var menu = Scenes.Instantiate<MainMenuView>();
        layer.AddChild(menu);

        menu.TestButton.Pressed += OnTestButtonPressed;
        menu.LobbyButton.Pressed += OnLobbyButtonPressed;
        menu.EditorButton.Pressed += OnEditorButtonPressed;
        menu.QuitButton.Pressed += OnQuitButtonPressed;
        
        this.AddElement(menu);
    }

    void OnLobbyButtonPressed()
    {
    }
    
    void OnEditorButtonPressed()
    {
    }
    
    void OnTestButtonPressed()
    {
    }
    
    void OnQuitButtonPressed()
    {
        this.GetElement<SceneTree>().Quit();
    }
}

public class DisableMenuFeatureSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        this.GetElement<MainMenuView>().QueueFree();
    }
}