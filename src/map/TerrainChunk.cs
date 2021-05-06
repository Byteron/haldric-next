using Godot;
using Leopotam.Ecs;

public struct EdgeVertices
{
    public Vector3 v1, v2, v3, v4, v5;

    public EdgeVertices(Vector3 corner1, Vector3 corner2)
    {
        v1 = corner1;
        v2 = corner1.Lerp(corner2, 0.25f);
        v3 = corner1.Lerp(corner2, 0.5f);
        v4 = corner1.Lerp(corner2, 0.75f);
        v5 = corner2;
    }

    public EdgeVertices(Vector3 corner1, Vector3 corner2, float outerStep)
    {
        v1 = corner1;
        v2 = corner1.Lerp(corner2, outerStep);
        v3 = corner1.Lerp(corner2, 0.5f);
        v4 = corner1.Lerp(corner2, 1f - outerStep);
        v5 = corner2;
    }
}

public partial class TerrainChunk : Node3D
{
    private static Color Weight1 = new Color(1f, 0f, 0f);
    private static Color Weight2 = new Color(0f, 1f, 0f);
    private static Color Weight3 = new Color(0f, 0f, 1f);

    private bool _enabled = false;

    private Locations _locations;

    private TerrainMesh _terrain;
    private TerrainFeaturePopulator _features;

    public override void _Ready()
    {
        _terrain = GetNode<TerrainMesh>("TerrainMesh");
        _features = GetNode<TerrainFeaturePopulator>("Features");
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
        ref var elevation = ref locEntity.Get<Elevation>();
        ref var plateauArea = ref locEntity.Get<PlateauArea>();

        Vector3 center = locEntity.Get<Coords>().World;
        center.y = elevation.Height;

        EdgeVertices e = new EdgeVertices(
            center + Metrics.GetFirstSolidCorner(direction, plateauArea),
            center + Metrics.GetSecondSolidCorner(direction, plateauArea)
        );

        TriangulatePlateau(center, e);

        if (direction <= Direction.W)
        {
            TriangulateConnection(direction, locEntity, e);
        }

        if (locEntity.Has<Forest>())
        {
            _features.AddFeature(center);
        }
    }

    private void TriangulateConnection(Direction direction, EcsEntity locEntity, EdgeVertices e1)
    {
        ref var coords = ref locEntity.Get<Coords>();
        ref var elevation = ref locEntity.Get<Elevation>();
        ref var neighbors = ref locEntity.Get<Neighbors>();

        if (!neighbors.Has(direction))
        {
            return;
        }

        var nLocEntity = neighbors.Get(direction);

        ref var nCoords = ref nLocEntity.Get<Coords>();
        ref var nElevation = ref nLocEntity.Get<Elevation>();
        ref var nPlateauArea = ref nLocEntity.Get<PlateauArea>();

        var bridge = Metrics.GetBridge(direction, nPlateauArea);
        bridge.y = (nCoords.World.y + nElevation.Height) - (coords.World.y + elevation.Height);

        var e2 = new EdgeVertices(
            e1.v1 + bridge,
            e1.v5 + bridge
        );

        TriangulateSlope(e1, e2);

        if (direction <= Direction.SW && neighbors.Has(direction.Next()))
        {
            var nextLocEntity = neighbors.Get(direction.Next());

            ref var nextElevation = ref nextLocEntity.Get<Elevation>();
            ref var nextPlateauArea = ref nextLocEntity.Get<PlateauArea>();

            var v6 = e1.v5 + Metrics.GetBridge(direction.Next(), nextPlateauArea);
            v6.y = nextElevation.Height;

            if (elevation.Level <= nElevation.Level)
            {
                if (elevation.Level <= nElevation.Level)
                {
                    TriangulateCorner(e1.v5, e2.v5, v6);
                }
                else
                {
                    TriangulateCorner(v6, e1.v5, e2.v5);
                }
            }
            else if (nElevation.Level <= nextElevation.Level)
            {
                TriangulateCorner(e2.v5, v6, e1.v5);
            }
            else
            {
                TriangulateCorner(v6, e1.v5, e2.v5);
            }
        }
    }

    private void TriangulateCorner(Vector3 bottom, Vector3 left, Vector3 right)
    {
        _terrain.AddTrianglePerturbed(left, bottom, right);
        // _terrain.AddTriangle(left, bottom, right);
    }

    private void TriangulatePlateau(Vector3 center, EdgeVertices edge)
    {
        _terrain.AddTrianglePerturbed(edge.v1, center, edge.v2);
        _terrain.AddTrianglePerturbed(edge.v2, center, edge.v3);
        _terrain.AddTrianglePerturbed(edge.v3, center, edge.v4);
        _terrain.AddTrianglePerturbed(edge.v4, center, edge.v5);

        // _terrain.AddTriangle(edge.v1, center, edge.v2);
        // _terrain.AddTriangle(edge.v2, center, edge.v3);
        // _terrain.AddTriangle(edge.v3, center, edge.v4);
        // _terrain.AddTriangle(edge.v4, center, edge.v5);
    }

    void TriangulateSlope(EdgeVertices e1, EdgeVertices e2)
    {   
        _terrain.AddQuadPerturbed(e1.v1, e1.v2, e2.v1, e2.v2);
        _terrain.AddQuadPerturbed(e1.v2, e1.v3, e2.v2, e2.v3);
        _terrain.AddQuadPerturbed(e1.v3, e1.v4, e2.v3, e2.v4);
        _terrain.AddQuadPerturbed(e1.v4, e1.v5, e2.v4, e2.v5);

        // _terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        // _terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        // _terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        // _terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);

        // _terrain.AddQuadUnperturbed(e1.v1, e1.v2, e2.v1, e2.v2);
        // _terrain.AddQuadUnperturbed(e1.v2, e1.v3, e2.v2, e2.v3);
        // _terrain.AddQuadUnperturbed(e1.v3, e1.v4, e2.v3, e2.v4);
        // _terrain.AddQuadUnperturbed(e1.v4, e1.v5, e2.v4, e2.v5);
    }
}