using Godot;
using System;

public partial class DebugView : CanvasLayer
{
    public Label StatsLabel;

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("debug"))
        {
            StatsLabel.Visible = !StatsLabel.Visible;
        }
    }

    public override void _Ready()
    {
        StatsLabel = GetNode<Label>("VBoxContainer/StatsLabel");
        StatsLabel.Visible = false;
    }
}
