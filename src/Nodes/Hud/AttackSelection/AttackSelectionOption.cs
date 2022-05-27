using RelEcs;
using RelEcs.Godot;
using Godot;

public partial class AttackSelectionOption : Button
{
    public string AttackerText { get; set; }
    public string DefenderText { get; set; }

    public Entity AttackerAttackEntity { get; set; }
    public Entity DefenderAttackEntity { get; set; }

     Label _attackerLabel;
     Label _defenderLabel;

    public override void _Ready()
    {
        _attackerLabel = GetNode<Label>("HBoxContainer/AttackerLabel");
        _defenderLabel = GetNode<Label>("HBoxContainer/DefenderLabel");

        _attackerLabel.Text = AttackerText;
        _defenderLabel.Text = DefenderText;
    }
}
