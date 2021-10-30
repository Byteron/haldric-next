using System;
using System.Collections.Generic;
using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public partial class RecruitSelectionView : Control
{
    [Export] PackedScene RecruitSelectionOption;


    private ButtonGroup _buttonGroup = new ButtonGroup();
    
    private int _side;
    private EcsEntity _locEntity;
    private RecruitSelectionOption _selectedOption;

    private Label _unitLabel;

    private VBoxContainer _container;
    private Button _acceptButton;

    public override void _Ready()
    {
        _acceptButton = GetNode<Button>("PanelContainer/VBoxContainer/Buttons/AcceptButton");
        _acceptButton.Disabled = true;
        _container = GetNode<VBoxContainer>("PanelContainer/VBoxContainer/HBoxContainer/OptionButtons");
        _unitLabel = GetNode<Label>("PanelContainer/VBoxContainer/HBoxContainer/UnitLabel");
    }

    public void UpdateInfo(EcsEntity locEntity, EcsEntity player, List<string> unitTypeIds)
    {
        _side = player.Get<Side>().Value;
        var gold = player.Get<Gold>().Value;
        _locEntity = locEntity;

        foreach (var unitTypeId in unitTypeIds)
        {
            var optionButton = RecruitSelectionOption.Instantiate<RecruitSelectionOption>();
            optionButton.Connect("pressed", new Callable(this, "OnRecruitOptionSelected"), new Godot.Collections.Array() { optionButton });
            optionButton.UnitType = Data.Instance.Units[unitTypeId].Instantiate<UnitType>();
            optionButton.Text = $"({optionButton.UnitType.Cost}) {unitTypeId}";
            optionButton.ButtonGroup = _buttonGroup;

            if (optionButton.UnitType.Cost > gold)
            {
                optionButton.Disabled = true;
            }
            else
            {
                _acceptButton.Disabled = false;
            }

            _container.AddChild(optionButton);
        }
        
        if (!_acceptButton.Disabled)
        {
            foreach(RecruitSelectionOption button in _container.GetChildren())
            {
                if (!button.Disabled)
                {
                    _selectedOption = button;
                    _selectedOption.Pressed = true;
                    OnRecruitOptionSelected(_selectedOption);
                    break;
                }
            }
        }
    }

    public void Cleanup()
    {
        foreach(RecruitSelectionOption button in _container.GetChildren())
        {
            button.UnitType.QueueFree();
        }
    }

    private void OnRecruitOptionSelected(RecruitSelectionOption optionButton)
    {
        _selectedOption = optionButton;
        string s = "";
        s += $"{optionButton.UnitType.Id}";
        s += $"\n\nL: {optionButton.UnitType.Level}";
        s += $"\nHP: {optionButton.UnitType.Health}";
        s += $"\nAP: {optionButton.UnitType.Actions}";
        s += $"\nMP: {optionButton.UnitType.Moves}";
        s += $"\n";

        foreach (Attack attack in optionButton.UnitType.GetNode<Node>("Attacks").GetChildren())
        {
            s += "\n" + attack.ToString();
        }

        _unitLabel.Text = s;
    }

    private void OnAcceptButtonPressed()
    {
        var recruitEvent = new RecruitUnitEvent(_side, _selectedOption.UnitType, _locEntity);
        Main.Instance.World.Spawn().Add(recruitEvent);

        _container.RemoveChild(_selectedOption);
        _selectedOption.QueueFree();

        var gameStateController = Main.Instance.World.GetResource<GameStateController>();
        gameStateController.PopState();
    }

    private void OnCancelButtonPressed()
    {
        var gameStateController = Main.Instance.World.GetResource<GameStateController>();
        gameStateController.PopState();
    }
}
