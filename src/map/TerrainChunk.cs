using Godot;
using Leopotam.Ecs;

public partial class TerrainChunk : Node3D
{
    private static Color Weight1 = new Color(1f, 0f, 0f);
    private static Color Weight2 = new Color(0f, 1f, 0f);
    private static Color Weight3 = new Color(0f, 0f, 1f);

    private bool _enabled = false;

    private Locations _locations;

    private TerrainMesh _terrain;

    public override void _Ready()
    {
        _terrain = GetNode<TerrainMesh>("TerrainMesh");
    }

    public void Build(Locations locations)
    {
        _locations = locations;
        _enabled = true;
        CallDeferred("Triangulate");
    }

    private void Triangulate()
    {
        if (_enabled)
        {
            Triangulate(_locations);
        }

        _enabled = false;
    }

    private void Triangulate(Locations locations)
    {
        _terrain.Clear();

        foreach (var item in locations.Dict)
        {
            Triangulate(item.Value);
        }

        _terrain.Apply();
    }

    private void Triangulate(EcsEntity locEntity)
    {
        for (Direction direction = Direction.NE; direction <= Direction.SE; direction++)
        {
            Triangulate(direction, locEntity);
        }
    }

    private void Triangulate(Direction direction, EcsEntity locEntity)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var elevation = ref locEntity.Get<Elevation>();
        ref var elevationStep = ref locEntity.Get<ElevationStep>();

        Vector3 center = coords.World;

        center.y = elevation.Value * elevationStep.Value;

        Vector3 v1 = center + Metrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + Metrics.GetSecondSolidCorner(direction);

        _terrain.AddTriangle(center, v1, v2);
        // _terrain.AddTriangleUnperturbed(center, v1, v2);

        TriangulateConnection(direction, locEntity, v1, v2);
    }

    private void TriangulateConnection(Direction direction, EcsEntity locEntity, Vector3 v1, Vector3 v2)
    {
        ref var neighbors = ref locEntity.Get<Neighbors>();

        if (!neighbors.Has(direction))
        {
            return;
        }

        var nLocEntity = neighbors.Get(direction);

        ref var nElevation = ref nLocEntity.Get<Elevation>();
        ref var nEevationStep = ref nLocEntity.Get<ElevationStep>();

        var v3 = v1 + Metrics.GetBridge(direction);
        var v4 = v2 + Metrics.GetBridge(direction);

        v3.y = v4.y = nElevation.Value * nEevationStep.Value;

        _terrain.AddQuad(v1, v2, v3, v4);
        // _terrain.AddQuadUnperturbed(v1, v2, v3, v4);

        if (neighbors.Has(direction.Next()) && direction <= Direction.SW)
        {
            var nextLocEntity = neighbors.Get(direction.Next());
            
            ref var nextElevation = ref nextLocEntity.Get<Elevation>();
            ref var nextEevationStep = ref nextLocEntity.Get<ElevationStep>();

            var v5 = v2 + Metrics.GetBridge(direction.Next());
            v5.y = nextElevation.Value * nextEevationStep.Value;
            _terrain.AddQuad(v1, v2, v3, v4);
            // _terrain.AddQuadUnperturbed(v1, v2, v3, v4);
            _terrain.AddTriangle(v2, v4, v5);
            // _terrain.AddTriangleUnperturbed(v2, v4, v5);
        }

    }
}