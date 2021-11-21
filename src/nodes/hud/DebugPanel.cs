using Godot;
using System;

public partial class DebugPanel : Control
{
    public Label StatsLabel { get; set; }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("debug"))
        {
            Visible = !Visible;
        }
    }

    public override void _Ready()
    {
        StatsLabel = GetNode<Label>("StatsLabel");
        Visible = false;
    }
}
