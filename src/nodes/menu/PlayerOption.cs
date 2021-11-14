using Godot;

public partial class PlayerOption : HBoxContainer
{
    [Signal] public delegate void FactionSelected(int side, int index);

    public int LocalPlayerId { get; set; }
    public int Side { get; set; }
    public string Faction { get => _options.GetItemText(_options.GetSelectedId()); }

    Label _label;
    OptionButton _options;

    public override void _Ready()
    {

        _label = GetNode<Label>("Label");
        _options = GetNode<OptionButton>("OptionButton");

        if (LocalPlayerId != Side)
        {
            _options.Disabled = true;
        }

        _label.Text = $"Player {Side} ";

        foreach (var faction in Data.Instance.Factions)
        {
            _options.AddItem(faction.Key);
        }

        if (_options.GetItemCount() > 0)
        {
            _options.Select(0);
        }
    }

    public void Select(int index)
    {
        _options.Select(index);
    }

    private void OnOptionButtonItemSelected(int index)
    {
        EmitSignal(nameof(FactionSelected), Side, index);
    }
}
