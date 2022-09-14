using Godot;
using RelEcs;

public partial class MainMenuView : PanelContainer
{
    [Signal] public delegate void LobbyButtonPressedEventHandler();
    [Signal] public delegate void TestButtonPressedEventHandler();
    [Signal] public delegate void EditorButtonPressedEventHandler();
    [Signal] public delegate void QuitButtonPressedEventHandler();

     void OnLobbyButtonPressed()
    {
        EmitSignal(nameof(LobbyButtonPressedEventHandler));
    }

     void OnTestButtonPressed()
    {
        EmitSignal(nameof(TestButtonPressedEventHandler));
    }

     void OnEditorButtonPressed()
    {
        EmitSignal(nameof(EditorButtonPressedEventHandler));
    }

    public void OnQuitButtonPressed()
    {
        EmitSignal(nameof(QuitButtonPressedEventHandler));
        GetTree().Quit();
    }
}
