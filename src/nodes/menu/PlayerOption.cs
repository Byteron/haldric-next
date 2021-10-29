using Godot;

public partial class PlayerOption : HBoxContainer
{
    public int Team { get; set; }
    public string Faction { get => _options.GetItemText(_options.GetSelectedId()); }

    Label _label;
    OptionButton _options;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _options = GetNode<OptionButton>("OptionButton");

        _label.Text = $"Player {Team} ";

        foreach (var faction in Data.Instance.Factions)
        {
            _options.AddItem(faction.Key);
        }

        if (_options.GetItemCount() > 0)
        {
            _options.Select(0);
        }
    }
}
