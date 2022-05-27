using Godot;
using RelEcs;

public partial class MenuState : GameState
{
    public override void Init(GameStateController gameStates)
    {
        InitSystems.Add(new MenuStateInitSystem());
        ContinueSystems.Add(new MenuStateContinueSystem());
        PauseSystems.Add(new MenuStatePauseSystem());
        ExitSystems.Add(new MenuStateExitSystem());
        
        UpdateSystems.Add(new UpdateStatsInfoSystem());
    }
}

public class MenuStateContinueSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.GetElement<MainMenuView>().Show();
    }
}

public class MenuStatePauseSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.GetElement<MainMenuView>().Hide();
    }
}

public class MenuStateExitSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.RemoveElement<MainMenuView>();
        commands.RemoveElement<DebugPanel>();
    }
}

public partial class MenuStateInitSystem : Resource, ISystem
{
    Commands _commands;

    public void Run(Commands commands)
    {
        _commands = commands;

        var canvas = commands.GetElement<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(10);
        
        var menuView = Scenes.Instantiate<MainMenuView>();

        menuView.Connect(nameof(MainMenuView.LobbyButtonPressed), new Callable(this, nameof(OnLobbyButtonPressed)));
        menuView.Connect(nameof(MainMenuView.TestButtonPressed), new Callable(this, nameof(OnTestButtonPressed)));
        menuView.Connect(nameof(MainMenuView.EditorButtonPressed), new Callable(this, nameof(OnEditorButtonPressed)));
        menuView.Connect(nameof(MainMenuView.QuitButtonPressed), new Callable(this, nameof(OnQuitButtonPressed)));

        var debugView = Scenes.Instantiate<DebugPanel>();

        canvasLayer.AddChild(menuView);
        canvasLayer.AddChild(debugView);

        commands.AddElement(menuView);
        commands.AddElement(debugView);
    }

    void OnLobbyButtonPressed()
    {
        _commands.GetElement<GameStateController>().PushState(new LoginState());
    }

     void OnTestButtonPressed()
    {
        _commands.GetElement<GameStateController>().PushState(new TestMapSelectionState());
    }

     void OnEditorButtonPressed()
    {
        _commands.GetElement<GameStateController>().PushState(new EditorState());
    }

    public void OnQuitButtonPressed()
    {
        _commands.GetElement<SceneTree>().Quit();
    }
}