using Godot;

public partial class AttackSelectionOption : Button
{
    public string AttackerText { get; set; }
    public string DefenderText { get; set; }

    private Label _attackerLabel;
    private Label _defenderLabel;

    public override void _Ready()
    {
        _attackerLabel = GetNode<Label>("HBoxContainer/AttackerLabel");
        _defenderLabel = GetNode<Label>("HBoxContainer/DefenderLabel");

        _attackerLabel.Text = AttackerText;
        _defenderLabel.Text = DefenderText;
    }
}
