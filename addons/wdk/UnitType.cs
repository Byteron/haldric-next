using Godot;
using System;

public partial class UnitType : Node3D
{
    public Attributes Attributes;
    public UnitView UnitView;

    public override void _Ready()
    {
        Attributes = GetNode<Attributes>("Attributes");
        UnitView = GetNode<UnitView>("UnitView");
    }
}
