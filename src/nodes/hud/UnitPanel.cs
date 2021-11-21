using Godot;
using System;

public partial class UnitPanel : Control
{
    Label _label;

    public override void _Ready()
    {
        _label = GetNode<Label>("PanelContainer/Label");
    }

    public void UpdateInfo(string text)
    {
        Hide();

        if (!string.IsNullOrEmpty(text))
        {
            Show();
            _label.Text = text;
        }
    }
}
