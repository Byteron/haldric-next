using Godot;
using Bitron.Ecs;

public partial class MainMenuView : PanelContainer
{
    [Signal] public delegate void LobbyButtonPressed();
    [Signal] public delegate void TestButtonPressed();
    [Signal] public delegate void EditorButtonPressed();
    [Signal] public delegate void QuitButtonPressed();

    private void OnLobbyButtonPressed()
    {
        EmitSignal(nameof(LobbyButtonPressed));
    }

    private void OnTestButtonPressed()
    {
        EmitSignal(nameof(TestButtonPressed));
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
