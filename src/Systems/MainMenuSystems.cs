using RelEcs;
using Godot;

public static class MainMenuSystems
{
    public static void SpawnMainMenu(World world)
    {
        var canvas = world.GetElement<Canvas>();
        var layer = canvas.GetCanvasLayer(1);

        var menu = Scenes.Instantiate<MainMenuView>();
        layer.AddChild(menu);

        menu.TestButton.Pressed += () =>
        {
            world.DisableState<MenuState>();
            world.EnableState<TestState>();
        };

        menu.LobbyButton.Pressed += () => { };

        menu.EditorButton.Pressed += () =>
        {
            world.DisableState<MenuState>();
            world.EnableState<EditorState>();
        };

        menu.QuitButton.Pressed += () => { world.DisableState<AppState>(); };

        world.AddElement(menu);
    }

    public static void DespawnMainMenu(World world)
    {
        var menu = world.GetElement<MainMenuView>();
        // TODO: figure out if this is necessary
        // menu.TestButton.Pressed -= world.OnTestButtonPressed;
        // menu.LobbyButton.Pressed -= world.OnLobbyButtonPressed;
        // menu.EditorButton.Pressed -= world.OnEditorButtonPressed;
        // menu.QuitButton.Pressed -= world.OnQuitButtonPressed;
        world.RemoveElement<MainMenuView>();
        menu.QueueFree();
    }
}