using Godot;
using System;

public partial class HUDView : CanvasLayer
{
    public Label TerrainLabel;
    public Label UnitLabel;
    public override void _Ready()
    {
        TerrainLabel = GetNode<Label>("VBoxContainer/TerrainLabel");
        UnitLabel = GetNode<Label>("VBoxContainer/UnitLabel");
    }
}
