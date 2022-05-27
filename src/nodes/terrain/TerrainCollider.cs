using Godot;
using System;

public partial class TerrainCollider : StaticBody3D
{
    CollisionShape3D _collisionShape = new();

    public TerrainCollider() { Name = "TerrainCollider"; }

    public override void _Ready() { AddChild(_collisionShape); }

    public void UpdateCollisionShape(Shape3D shape) { _collisionShape.Shape = shape; }
}