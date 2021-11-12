using Godot;
using Bitron.Ecs;

public partial class MainMenuView : PanelContainer
{
    [Signal] delegate void PlayButtonPressed();
    [Signal] delegate void LobbyButtonPressed();
    [Signal] delegate void MatchButtonPressed();
    [Signal] delegate void EditorButtonPressed();
    [Signal] delegate void QuitButtonPressed();

    private void OnPlayButtonPressed()
    {
        EmitSignal(nameof(PlayButtonPressed));
    }

    private void OnLobbyButtonPressed()
    {
        EmitSignal(nameof(LobbyButtonPressed));
    }

    private void OnMatchButtonPressed()
    {
        EmitSignal(nameof(MatchButtonPressed));
    }

    private void OnEditorButtonPressed()
    {
        EmitSignal(nameof(EditorButtonPressed));
    }

    public void OnQuitButtonPressed()
    {   
        EmitSignal(nameof(QuitButtonPressed));
        GetTree().Quit();
    }
}