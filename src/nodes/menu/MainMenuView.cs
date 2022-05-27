using Godot;
using RelEcs;
using RelEcs.Godot;

public partial class MainMenuView : PanelContainer
{
    [Signal] public delegate void LobbyButtonPressed();
    [Signal] public delegate void TestButtonPressed();
    [Signal] public delegate void EditorButtonPressed();
    [Signal] public delegate void QuitButtonPressed();

     void OnLobbyButtonPressed()
    {
        EmitSignal(nameof(LobbyButtonPressed));
    }

     void OnTestButtonPressed()
    {
        EmitSignal(nameof(TestButtonPressed));
    }

     void OnEditorButtonPressed()
    {
        EmitSignal(nameof(EditorButtonPressed));
    }

    public void OnQuitButtonPressed()
    {
        EmitSignal(nameof(QuitButtonPressed));
        GetTree().Quit();
    }
}
