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
        var menuView = Scenes.Instance.MainMenuView.Instantiate<MainMenuView>();

        menuView.Connect("PlayButtonPressed", new Callable(this, nameof(OnPlayButtonPressed)));
        menuView.Connect("LobbyButtonPressed", new Callable(this, nameof(OnLobbyButtonPressed)));
        menuView.Connect("MatchButtonPressed", new Callable(this, nameof(OnMatchButtonPressed)));
        menuView.Connect("EditorButtonPressed", new Callable(this, nameof(OnEditorButtonPressed)));
        menuView.Connect("QuitButtonPressed", new Callable(this, nameof(OnQuitButtonPressed)));

        AddChild(menuView);

        var debugView = Scenes.Instance.DebugView.Instantiate<DebugView>();
        AddChild(debugView);

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

    private void OnPlayButtonPressed()
    {
        _world.GetResource<GameStateController>().PushState(new ScenarioSelectionState(_world));
    }

    private void OnLobbyButtonPressed()
    {
        _world.GetResource<GameStateController>().PushState(new LobbyState(_world));
    }

    private void OnMatchButtonPressed()
    {
        _world.GetResource<GameStateController>().PushState(new ScenarioSelectionState(_world));
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