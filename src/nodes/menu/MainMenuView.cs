using Godot;
using Bitron.Ecs;

public partial class MainMenuView : PanelContainer
{
    private void OnPlayButtonPressed()
    {
        Main.Instance.World.GetResource<GameStateController>().PushState(new ScenarioSelectionState(Main.Instance.World));
    }

    private void OnLobbyButtonPressed()
    {
        Main.Instance.World.GetResource<GameStateController>().PushState(new LobbyState(Main.Instance.World));
    }

    private void OnMatchButtonPressed()
    {
        Main.Instance.World.GetResource<GameStateController>().PushState(new MatchState(Main.Instance.World));
    }

    private void OnEditorButtonPressed()
    {
        Main.Instance.World.GetResource<GameStateController>().PushState(new EditorState(Main.Instance.World));
    }

    public void OnQuitButtonPressed()
    {   
        GetTree().Quit();
    }
}