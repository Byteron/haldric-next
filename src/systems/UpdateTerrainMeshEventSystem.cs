using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public class UpdateTerrainMeshEvent
{
    public List<Vector3i> Chunks { get; }

    public UpdateTerrainMeshEvent(List<Vector3i> chunks = null)
    {
        Chunks = chunks;
    }
}

public class EdgeVertices
{
    public Vector3 V1, V2, V3, V4, V5;

    public EdgeVertices(Vector3 corner1, Vector3 corner2)
    {
        V1 = corner1;
        V2 = corner1.Lerp(corner2, 0.25f);
        V3 = corner1.Lerp(corner2, 0.5f);
        V4 = corner1.Lerp(corner2, 0.75f);
        V5 = corner2;
    }

    public EdgeVertices(Vector3 corner1, Vector3 corner2, float outerStep)
    {
        V1 = corner1;
        V2 = corner1.Lerp(corner2, outerStep);
        V3 = corner1.Lerp(corner2, 0.5f);
        V4 = corner1.Lerp(corner2, 1f - outerStep);
        V5 = corner2;
    }
}

public class UpdateTerrainMeshEventSystem : ISystem
{
    static readonly Color ColorRed = new (1.0f, 0f, 0f);
    static readonly Color ColorGreen = new (0f, 1.0f, 0f);
    static readonly Color ColorBlue = new (0f, 0f, 1.0f);

    TerrainMesh _terrainMesh;
    ShaderData _shaderData;

    public void Run(Commands commands)
    {
        var chunkQuery = commands.Query<Locations, TerrainMesh, TerrainCollider, Cell>();

        commands.Receive((UpdateTerrainMeshEvent e) =>
        {
            _shaderData = commands.GetElement<ShaderData>();

            foreach (var (locs, mesh, collider, cell) in chunkQuery)
            {
                if (e.Chunks != null && !e.Chunks.Contains(cell.Value)) continue;

                _terrainMesh = mesh;

                Triangulate(locs);
                
                collider.UpdateCollisionShape(_terrainMesh.Mesh.CreateTrimeshShape());
            }

            _shaderData.Apply();
        });
    }

    void Triangulate(Locations locations)
    {
        _terrainMesh.Clear();

        foreach (var locEntity in locations.Dict.Values)
        {
            Triangulate(locEntity);

            var coords = locEntity.Get<Coords>();
            var baseTerrain = locEntity.Get<HasBaseTerrain>();

            var x = (int)coords.Offset().x;
            var z = (int)coords.Offset().z;

            _shaderData.UpdateTerrain(x, z, baseTerrain.Entity.Get<TerrainTypeIndex>().Value);

            if (baseTerrain.Entity.Has<NoLighting>())
            {
                _shaderData.UpdateLighting(x, z, false);
            }
        }

        _terrainMesh.Apply();
    }

    void Triangulate(Entity locEntity)
    {
        for (var i = 0; i < 6; i++)
        {
            Triangulate((Direction)i, locEntity);
        }
    }

    void Triangulate(Direction direction, Entity locEntity)
    {
        var index = locEntity.Get<Index>();

        var elevation = locEntity.Get<Elevation>();
        var plateauArea = locEntity.Get<PlateauArea>();

        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        var elevationOffset = terrainEntity.Get<ElevationOffset>();

        var center = locEntity.Get<Coords>().World();
        center.y = elevation.Height + elevationOffset.Value;

        var e = new EdgeVertices(
            center + Metrics.GetFirstSolidCorner(direction, plateauArea),
            center + Metrics.GetSecondSolidCorner(direction, plateauArea)
        );

        TriangulatePlateau(center, e, index.Value);

        if (direction <= (Direction)2)
        {
            TriangulateConnection(direction, locEntity, e);
        }
    }

    void TriangulateConnection(Direction direction, Entity locEntity, EdgeVertices e1)
    {
        var locIndex = locEntity.Get<Index>().Value;

        var coords = locEntity.Get<Coords>();
        var elevation = locEntity.Get<Elevation>();
        var neighbors = locEntity.Get<Neighbors>();

        var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
        var elevationOffset = terrainEntity.Get<ElevationOffset>();

        if (!neighbors.Has(direction))
        {
            return;
        }

        var nLocEntity = neighbors.Get(direction);
        var nLocIndex = nLocEntity.Get<Index>();

        var nCoords = nLocEntity.Get<Coords>();
        var nElevation = nLocEntity.Get<Elevation>();
        var nPlateauArea = nLocEntity.Get<PlateauArea>();

        var nTerrainEntity = nLocEntity.Get<HasBaseTerrain>().Entity;
        var nElevationOffset = nTerrainEntity.Get<ElevationOffset>();

        var bridge = Metrics.GetBridge(direction, nPlateauArea);
        bridge.y = (nCoords.World().y + nElevation.Height + nElevationOffset.Value) - (coords.World().y + elevation.Height + elevationOffset.Value);

        var e2 = new EdgeVertices(
            e1.V1 + bridge,
            e1.V5 + bridge
        );

        TriangulateSlope(e1, e2, locIndex, nLocIndex.Value);

        if (direction > (Direction)2 || !neighbors.Has(direction.Next())) return;
        
        var nextLocEntity = neighbors.Get(direction.Next());
        var nextLocIndex = nextLocEntity.Get<Index>();

        var nextElevation = nextLocEntity.Get<Elevation>();
        var nextPlateauArea = nextLocEntity.Get<PlateauArea>();

        var nextTerrainEntity = nextLocEntity.Get<HasBaseTerrain>().Entity;
        var nextElevationOffset = nextTerrainEntity.Get<ElevationOffset>();

        var v6 = e1.V5 + Metrics.GetBridge(direction.Next(), nextPlateauArea);
        v6.y = nextElevation.Height + nextElevationOffset.Value;

        var indices = new Vector3();

        if (elevation.Value <= nElevation.Value)
        {
            indices.x = locIndex;
            indices.y = nLocIndex.Value;
            indices.z = nextLocIndex.Value;
            TriangulateCorner(e1.V5, e2.V5, v6, indices);
        }
        else if (nElevation.Value <= nextElevation.Value)
        {
            indices.x = nLocIndex.Value;
            indices.y = nextLocIndex.Value;
            indices.z = locIndex;
            TriangulateCorner(e2.V5, v6, e1.V5, indices);
        }
        else
        {
            indices.x = nextLocIndex.Value;
            indices.y = locIndex;
            indices.z = nLocIndex.Value;
            TriangulateCorner(v6, e1.V5, e2.V5, indices);
        }
    }

    void TriangulateCorner(Vector3 bottom, Vector3 left, Vector3 right, Vector3 indices)
    {
        _terrainMesh.AddTrianglePerturbed(bottom, left, right);
        _terrainMesh.AddTriangleCellData(indices, ColorRed, ColorGreen, ColorBlue);
    }

    void TriangulatePlateau(Vector3 center, EdgeVertices edge, float index)
    {
        // _terrainMesh.AddTrianglePerturbed(edge.v1, center, edge.v5);
        _terrainMesh.AddTrianglePerturbed(edge.V2, center, edge.V1);
        _terrainMesh.AddTrianglePerturbed(edge.V3, center, edge.V2);
        _terrainMesh.AddTrianglePerturbed(edge.V4, center, edge.V3);
        _terrainMesh.AddTrianglePerturbed(edge.V5, center, edge.V4);
        _terrainMesh.AddTriangleCellData(new Vector3(index, index, index), ColorRed);
        _terrainMesh.AddTriangleCellData(new Vector3(index, index, index), ColorRed);
        _terrainMesh.AddTriangleCellData(new Vector3(index, index, index), ColorRed);
        _terrainMesh.AddTriangleCellData(new Vector3(index, index, index), ColorRed);
    }

    void TriangulateSlope(EdgeVertices e1, EdgeVertices e2, float index1, float index2)
    {
        // _terrainMesh.AddQuadPerturbed(e1.v1, e1.v5, e2.v1, e2.v5);
        _terrainMesh.AddQuadPerturbed(e1.V2, e1.V1, e2.V2, e2.V1);
        _terrainMesh.AddQuadPerturbed(e1.V3, e1.V2, e2.V3, e2.V2);
        _terrainMesh.AddQuadPerturbed(e1.V4, e1.V3, e2.V4, e2.V3);
        _terrainMesh.AddQuadPerturbed(e1.V5, e1.V4, e2.V5, e2.V4);
        _terrainMesh.AddQuadCellData(new Vector3(index1, index2, index1), ColorRed, ColorGreen);
        _terrainMesh.AddQuadCellData(new Vector3(index1, index2, index1), ColorRed, ColorGreen);
        _terrainMesh.AddQuadCellData(new Vector3(index1, index2, index1), ColorRed, ColorGreen);
        _terrainMesh.AddQuadCellData(new Vector3(index1, index2, index1), ColorRed, ColorGreen);
    }
}