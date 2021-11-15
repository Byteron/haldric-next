using Godot;

public partial class ScenarioSelectionView : PanelContainer
{
    [Signal] public delegate void ContinuePressed(string mapName);
    [Signal] public delegate void CancelPressed();

    private OptionButton _options;

    public override void _Ready()
    {
        _options = GetNode<OptionButton>("CenterContainer/VBoxContainer/OptionButton");

        foreach (var pair in Data.Instance.Maps)
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
        EmitSignal(nameof(ContinuePressed), mapName);
    }

    private void OnBackButtonPressed()
    {
        EmitSignal(nameof(CancelPressed));
    }
}
