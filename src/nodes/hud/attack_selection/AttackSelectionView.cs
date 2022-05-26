using System;
using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Godot;

public partial class AttackSelectionView : Control
{
    [Signal] public delegate void AttackSelected();
    [Signal] public delegate void CancelButtonPressed();

    [Export] PackedScene AttackSelectionOption;

     ButtonGroup _buttonGroup = new();

     AttackSelectionOption _selectedOption;

     Label _attackerLabel;
     Label _defenderLabel;

     Button _acceptButton;

     VBoxContainer _container;

    public override void _Ready()
    {
        _acceptButton = GetNode<Button>("PanelContainer/VBoxContainer/Buttons/AcceptButton");
        _container = GetNode<VBoxContainer>("PanelContainer/VBoxContainer/OptionButtons");
        _attackerLabel = GetNode<Label>("PanelContainer/VBoxContainer/UnitInfo/AttackerLabel");
        _defenderLabel = GetNode<Label>("PanelContainer/VBoxContainer/UnitInfo/DefenderLabel");
    }

    public Entity GetSelectedAttackerAttack()
    {
        return _selectedOption.AttackerAttackEntity;
    }

    public Entity GetSelectedDefenderAttack()
    {
        return _selectedOption.DefenderAttackEntity;
    }

    public void UpdateInfo(Entity attackerLocEntity, Entity defenderLocEntity, Dictionary<Entity, Entity> attackPairs)
    {
        _attackerLabel.Text = $"{attackerLocEntity.Get<HasUnit>().Entity.Get<Id>().Value}";
        _defenderLabel.Text = $"{defenderLocEntity.Get<HasUnit>().Entity.Get<Id>().Value}";

        foreach (var attackPair in attackPairs)
        {
            var optionButton = AttackSelectionOption.Instantiate<AttackSelectionOption>();
            optionButton.Connect("pressed", new Callable(this, "OnAttackOptionSelected"), new Godot.Collections.Array() { optionButton });
            optionButton.AttackerAttackEntity = attackPair.Key;
            optionButton.DefenderAttackEntity = attackPair.Value;
            optionButton.ButtonGroup = _buttonGroup;
            optionButton.AttackerText = AttackToString(attackPair.Key);

            optionButton.DefenderText = attackPair.Value.IsAlive ? AttackToString(attackPair.Value) : " - ";

            _container.AddChild(optionButton);
        }

        _selectedOption = _container.GetChild<AttackSelectionOption>(0);
        _selectedOption.ButtonPressed = true;
    }

     void OnAttackOptionSelected(AttackSelectionOption optionButton)
    {
        _selectedOption = optionButton;
    }

     void OnAcceptButtonPressed()
    {
        _acceptButton.Disabled = true;
        EmitSignal(nameof(AttackSelected));
    }

     void OnCancelButtonPressed()
    {
        EmitSignal(nameof(CancelButtonPressed));
    }

     string AttackToString(Entity attackEntity)
    {
        var s = "";
        var attackId = attackEntity.Get<Id>();

        var damage = attackEntity.Get<Damage>();
        var strikes = attackEntity.Get<Strikes>();
        var range = attackEntity.Get<Range>();
        s += string.Format("{0} {1}x{2}~{4} ({3})", attackId.Value, damage.Value, strikes.Value, damage.Type.ToString(), range.Value.ToString());
        return s;
    }
}
