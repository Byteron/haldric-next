using Godot;
using Bitron.Ecs;

public partial class ScenarioSelectionView : PanelContainer
{
    OptionButton _options;

    public override void _Ready()
    {
        _options = GetNode<OptionButton>("CenterContainer/VBoxContainer/OptionButton");

        foreach(var pair in Data.Instance.Maps)
        {
            _options.AddItem(pair.Key);
        }

        if (_options.GetItemCount() > 0)
        {
            _options.Select(0);
        }
    }

    private void OnContinueButtonPressed()
    {
        var mapName = _options.GetItemText(_options.GetSelectedId());
        var gameStateController = Main.Instance.World.GetResource<GameStateController>();
        gameStateController.PopState();
        gameStateController.PushState(new FactionSelectionState(Main.Instance.World, mapName));
    }

    private void OnBackButtonPressed()
    {
        Main.Instance.World.GetResource<GameStateController>().PopState();
    }
}
