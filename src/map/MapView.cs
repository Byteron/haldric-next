using Godot;
using System;

public partial class MapView : Node3D
{
    public override void _Ready()
    {
        GD.Print("Ready!");
    }

    public void Build(Locations locations)
    {
        GD.Print("Building Map!: ", locations.GetDict().Count);

        foreach (var item in locations.GetDict())
        {
            var cell = item.Key;
            var entity = item.Value;

            var coords = Coords.FromCube(cell);

            var instance = new MeshInstance3D();
            instance.Mesh = new BoxMesh();
            AddChild(instance);

            instance.Translation = coords.World;
        }
    }
}
