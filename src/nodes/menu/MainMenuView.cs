using Godot;
using System;
using Bitron.Ecs;

public partial class MainMenuView : Control
{
    private void OnPlayButtonPressed()
    {
        Main.Instance.World.GetResource<GameStateController>().PushState(new PlayState(Main.Instance.World));
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
