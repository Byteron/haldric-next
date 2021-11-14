using Godot;
using System;

public partial class FlagView : Node3D
{
    public Color Color { get; set; }
    
    private MeshInstance3D _meshInstance;

    public override void _Ready()
    {
        _meshInstance = GetNode<MeshInstance3D>("Flag/flag_human_01");

        _meshInstance.MaterialOverride.Set("albedo_color", Color);
    }
}
