using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;

public partial class RecruitSelectionView : Control
{
    [Signal] public delegate void RecruitSelected(string unitTypeId);
    [Signal] public delegate void CancelButtonPressed();

    [Export] PackedScene RecruitSelectionOption;

     ButtonGroup _buttonGroup = new();

     RecruitSelectionOption _selectedOption;

     Label _unitLabel;

     VBoxContainer _container;
     Button _acceptButton;

    public override void _Ready()
    {
        _acceptButton = GetNode<Button>("PanelContainer/VBoxContainer/Buttons/AcceptButton");
        _acceptButton.Disabled = true;
        _container = GetNode<VBoxContainer>("PanelContainer/VBoxContainer/HBoxContainer/OptionButtons");
        _unitLabel = GetNode<Label>("PanelContainer/VBoxContainer/HBoxContainer/UnitLabel");
    }

    public void UpdateInfo(Entity locEntity, Entity sideEntity, List<string> unitTypeIds)
    {
        var side = sideEntity.Get<Side>().Value;
        var gold = sideEntity.Get<Gold>().Value;

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
            foreach (RecruitSelectionOption button in _container.GetChildren())
            {
                if (!button.Disabled)
                {
                    _selectedOption = button;
                    _selectedOption.ButtonPressed = true;
                    OnRecruitOptionSelected(_selectedOption);
                    break;
                }
            }
        }
    }

    public void Cleanup()
    {
        foreach (RecruitSelectionOption button in _container.GetChildren())
        {
            button.UnitType.QueueFree();
        }
    }

     void OnRecruitOptionSelected(RecruitSelectionOption optionButton)
    {
        _selectedOption = optionButton;
        var s = "";
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

     void OnAcceptButtonPressed()
    {
        _acceptButton.Disabled = true;
        EmitSignal(nameof(RecruitSelected), _selectedOption.UnitType.Name);
    }

     void OnCancelButtonPressed()
    {
        EmitSignal(nameof(CancelButtonPressed));
    }
}
