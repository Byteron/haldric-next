using Godot;
using System;

public partial class MainMenuView : Control
{
    private void OnPlayButtonPressed()
    {
        Main.Instance.GameStateController.ChangeState(new PlayState(Main.Instance.World));
    }

    private void OnEditorButtonPressed()
    {
        Main.Instance.GameStateController.ChangeState(new EditorState(Main.Instance.World));
    }

    public void OnQuitButtonPressed()
    {   
        GetTree().Quit();
    }
}
