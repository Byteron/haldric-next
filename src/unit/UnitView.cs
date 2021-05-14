using Godot;
using System;

public partial class UnitView : Node3D
{

    public override void _Ready()
    {
        if (GD.Randf() < 0.5)
        {
            GetNode<Node3D>("Infantry").Visible = true;
            GetNode<Node3D>("Unit").Visible = false;
        }
        else
        {
            GetNode<Node3D>("Infantry").Visible = false;
            GetNode<Node3D>("Unit").Visible = true;
        }
    }
}
