using Godot;
using System;

public partial class MainMenuView : Control
{
    private void OnPlayButtonPressed()
    {
        Main.Instance.GameStateController.PushState(new PlayState(Main.Instance.World));
    }

    private void OnEditorButtonPressed()
    {
        Main.Instance.GameStateController.PushState(new EditorState(Main.Instance.World));
    }

    public void OnQuitButtonPressed()
    {   
        GetTree().Quit();
    }
}
