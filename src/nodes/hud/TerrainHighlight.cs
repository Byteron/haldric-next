using Godot;
using System;

public partial class TerrainHighlight : MeshInstance3D
{
    public Color Color { get; set; } = new Color("FFFFFF");
    public float ScaleFactor { get; set; } = 1f;

    public override void _Ready()
    {
        Scale *= ScaleFactor;
        this.MaterialOverride.Set("albedo_color", Color);
    }
}
