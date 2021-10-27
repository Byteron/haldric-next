using System;
using System.Collections.Generic;
using Bitron.Ecs;
using Godot;

public partial class AttackSelectionView : Control
{
    [Export] PackedScene AttackSelectionOption;

    public EcsEntity AttackerLocEntity { get; set; }
    public EcsEntity DefenderLocEntity { get; set; }

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

    public void UpdateInfo(EcsEntity attackerLocEntity, EcsEntity defenderLocEntity, Dictionary<EcsEntity, EcsEntity> attackPairs)
    {
        AttackerLocEntity = attackerLocEntity;
        DefenderLocEntity = defenderLocEntity;

        _attackerLabel.Text = $"{AttackerLocEntity.Get<HasUnit>().Entity.Get<Id>().Value}";
        _defenderLabel.Text = $"{DefenderLocEntity.Get<HasUnit>().Entity.Get<Id>().Value}";
        
        foreach(var attackPair in attackPairs)
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
        var commander = Main.Instance.World.GetResource<Commander>();
        var gameStateController = Main.Instance.World.GetResource<GameStateController>();

        commander.Enqueue(new CombatCommand(AttackerLocEntity, _selectedOption.AttackerAttackEntity, DefenderLocEntity, _selectedOption.DefenderAttackEntity));
            
        gameStateController.PopState();
        gameStateController.PushState(new CommanderState(Main.Instance.World));
    }

    private void OnCancelButtonPressed()
    {
        var gameStateController = Main.Instance.World.GetResource<GameStateController>();
        gameStateController.PopState();
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
