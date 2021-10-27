using System.Collections.Generic;
using Bitron.Ecs;
using Godot;

public partial class AttackSelectionView : Control
{
    [Export] PackedScene AttackSelectionOption;

    Control _container;
    ButtonGroup _buttonGroup = new ButtonGroup();

    public override void _Ready()
    {
        _container = GetNode<Control>("PanelContainer/VBoxContainer/VBoxContainer");
    }

    public void UpdateInfo(List<EcsEntity> attacks)
    {
        foreach(var attackEntity in attacks)
        {
            string s = "";
            ref var attackId = ref attackEntity.Get<Id>();

            ref var damage = ref attackEntity.Get<Damage>();
            ref var strikes = ref attackEntity.Get<Strikes>();
            ref var range = ref attackEntity.Get<Range>();
            s += string.Format("\n{0} {1}x{2}~{4} ({3})", attackId.Value, damage.Value, strikes.Value, damage.Type.ToString(), range.Value.ToString());
            
            var optionButton = AttackSelectionOption.Instantiate<AttackSelectionOption>();
            optionButton.ButtonGroup = _buttonGroup;
            optionButton.AttackerText = s;
            _container.AddChild(optionButton);
        }
    }

    private void OnAcceptButtonPressed()
    {

    }

    private void OnCancelButtonPressed()
    {
        
    }
}
