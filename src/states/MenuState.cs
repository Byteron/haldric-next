using Bitron.Ecs;
using Godot;

public partial class MenuState : GameState
{
    public MenuState(EcsWorld world) : base(world)
    {
        AddUpdateSystem(new UpdateStatsInfoSystem());
    }

    public override void Enter(GameStateController gameStates)
    {
        var canvas = _world.GetResource<Canvas>();

        var menuView = Scenes.Instance.MainMenuView.Instantiate<MainMenuView>();

        menuView.Connect(nameof(MainMenuView.LobbyButtonPressed), new Callable(this, nameof(OnLobbyButtonPressed)));
        menuView.Connect(nameof(MainMenuView.TestButtonPressed), new Callable(this, nameof(OnTestButtonPressed)));
        menuView.Connect(nameof(MainMenuView.EditorButtonPressed), new Callable(this, nameof(OnEditorButtonPressed)));
        menuView.Connect(nameof(MainMenuView.QuitButtonPressed), new Callable(this, nameof(OnQuitButtonPressed)));

        AddChild(menuView);

        var canvasLayer = canvas.GetCanvasLayer(10);
        var debugView = Scenes.Instance.DebugView.Instantiate<DebugView>();
        canvasLayer.AddChild(debugView);

        _world.AddResource(menuView);
        _world.AddResource(debugView);
    }

    public override void Continue(GameStateController gameStates)
    {
        _world.GetResource<MainMenuView>().Show();
    }

    public override void Pause(GameStateController gameStates)
    {
        _world.GetResource<MainMenuView>().Hide();
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.RemoveResource<MainMenuView>();
        _world.RemoveResource<DebugView>();
    }

    private void OnLobbyButtonPressed()
    {
        _world.GetResource<GameStateController>().PushState(new LoginState(_world));
    }

    private void OnTestButtonPressed()
    {
        _world.GetResource<GameStateController>().PushState(new TestMapSelectionState(_world));
    }

    private void OnEditorButtonPressed()
    {
        _world.GetResource<GameStateController>().PushState(new EditorState(_world));
    }

    public void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}