using Godot;
using System;
using Leopotam.Ecs;

public partial class TerrainFeaturePopulator : Node3D
{
    [Export] Mesh _forestMesh;
    [Export] Mesh _wallMesh;
    [Export] Mesh _towerMesh;

    Node3D container = new Node3D();

    public override void _Ready()
    {
        Clear();
    }

    public void Clear()
    {
        container?.QueueFree();
        container = new Node3D();
        AddChild(container);
    }

    public void Apply()
    {

    }

    public void AddFeature(Vector3 position)
    {
        var forest = new MeshInstance3D();
        forest.GiMode = GeometryInstance3D.GIMode.Dynamic;
        forest.Mesh = _forestMesh;
        forest.Translation = position;
        container.AddChild(forest);
    }

    public void AddTower()
    {

    }

    public void AddWall()
    {
        
    }
}
