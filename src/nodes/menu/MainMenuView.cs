using Godot;
using Bitron.Ecs;

public partial class MainMenuView : PanelContainer
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
