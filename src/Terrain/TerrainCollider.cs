using Godot;

namespace Haldric;

public partial class TerrainCollider : StaticBody3D
{
    public CollisionShape3D CollisionShape = new();

    public TerrainCollider()
    {
        Name = "TerrainCollider";
    }

    public override void _Ready()
    {
        AddChild(CollisionShape);
    }
}