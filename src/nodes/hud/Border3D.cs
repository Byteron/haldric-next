using Godot;
using System;

public partial class Border3D : Node3D
{
    public Color Color { get; set; }
    public float Direction { get; set; }
	
    public override void _Ready()
    {
        MeshInstance3D mesh = this.GetChild<MeshInstance3D>(0);
        mesh.GetSurfaceOverrideMaterial(0).Set("albedo_color", Color);
        Rotate(Vector3.Down, Direction);
    }
}
