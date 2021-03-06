using Godot;

public partial class ScenarioSelectionView : PanelContainer
{
    [Signal] public delegate void ContinuePressed(string mapName);
    [Signal] public delegate void CancelPressed();

     OptionButton _options;

    public override void _Ready()
    {
        _options = GetNode<OptionButton>("CenterContainer/VBoxContainer/OptionButton");

        foreach (var pair in Data.Instance.Maps)
        {
            _options.AddItem(pair.Key);
        }

        if (_options.ItemCount > 0)
        {
            _options.Select(0);
        }
    }

     void OnContinueButtonPressed()
    {
        var mapName = _options.GetItemText(_options.GetSelectedId());
        EmitSignal(nameof(ContinuePressed), mapName);
    }

     void OnBackButtonPressed()
    {
        EmitSignal(nameof(CancelPressed));
    }
}
