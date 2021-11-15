using System;
using System.Collections.Generic;
using Bitron.Ecs;
using Godot;

public partial class AttackSelectionView : Control
{
    [Signal] public delegate void AttackSelected();
    [Signal] public delegate void CancelButtonPressed();

    [Export] PackedScene AttackSelectionOption;

    private ButtonGroup _buttonGroup = new ButtonGroup();

    private AttackSelectionOption _selectedOption;

    private Label _attackerLabel;
    private Label _defenderLabel;

    private VBoxContainer _container;

    public override void _Ready()
    {
        _container = GetNode<VBoxContainer>("PanelContainer/VBoxContainer/OptionButtons");
        _attackerLabel = GetNode<Label>("PanelContainer/VBoxContainer/UnitInfo/AttackerLabel");
        _defenderLabel = GetNode<Label>("PanelContainer/VBoxContainer/UnitInfo/DefenderLabel");
    }

    public EcsEntity GetSelectedAttackerAttack()
    {
        return _selectedOption.AttackerAttackEntity;
    }

    public EcsEntity GetSelectedDefenderAttack()
    {
        return _selectedOption.DefenderAttackEntity;
    }

    public void UpdateInfo(EcsEntity attackerLocEntity, EcsEntity defenderLocEntity, Dictionary<EcsEntity, EcsEntity> attackPairs)
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

            if (attackPair.Value.IsAlive())
            {
                optionButton.DefenderText = AttackToString(attackPair.Value);
            }
            else
            {
                optionButton.DefenderText = " - ";
            }

            _container.AddChild(optionButton);
        }

        _selectedOption = _container.GetChild<AttackSelectionOption>(0);
        _selectedOption.Pressed = true;
    }

    private void OnAttackOptionSelected(AttackSelectionOption optionButton)
    {
        _selectedOption = optionButton;
    }

    private void OnAcceptButtonPressed()
    {
        EmitSignal(nameof(AttackSelected));
    }

    private void OnCancelButtonPressed()
    {
        EmitSignal(nameof(CancelButtonPressed));
    }

    private string AttackToString(EcsEntity attackEntity)
    {
        string s = "";
        ref var attackId = ref attackEntity.Get<Id>();

        ref var damage = ref attackEntity.Get<Damage>();
        ref var strikes = ref attackEntity.Get<Strikes>();
        ref var range = ref attackEntity.Get<Range>();
        s += string.Format("{0} {1}x{2}~{4} ({3})", attackId.Value, damage.Value, strikes.Value, damage.Type.ToString(), range.Value.ToString());
        return s;
    }
}
