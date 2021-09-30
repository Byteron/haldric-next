using Godot;
using System;

public partial class DebugView : CanvasLayer
{
    public Label StatsLabel;
    public override void _Ready()
    {
        StatsLabel = GetNode<Label>("VBoxContainer/StatsLabel");
    }
}
