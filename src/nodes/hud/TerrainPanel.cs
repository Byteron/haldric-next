using Godot;
using System;

public partial class TerrainPanel : Control
{
    Label _label;

    public override void _Ready()
    {
        _label = GetNode<Label>("PanelContainer/Label");
    }

    public void UpdateInfo(string text)
    {
        _label.Text = text;
    }
}
