using System.Collections.Generic;
using Godot;
using Bitron.Ecs;

public struct UpdateTerrainMeshEvent
{
    public List<Vector3i> Chunks;

    public UpdateTerrainMeshEvent(List<Vector3i> chunks = null)
    {
        Chunks = chunks;
    }
}

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

public class UpdateTerrainMeshEventSystem : IEcsSystem
{
    public static readonly Color ColorRed = new Color(1.0f, 0f, 0f);
    public static readonly Color ColorGreen = new Color(0f, 1.0f, 0f);
    public static readonly Color ColorBlue = new Color(0f, 0f, 1.0f);

    TerrainMesh _terrainMesh;
    ShaderData _shaderData;

    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<UpdateTerrainMeshEvent>().End();
        var chunkQuery = world.Query<Locations>().Inc<NodeHandle<TerrainMesh>>().Inc<NodeHandle<TerrainCollider>>().End();

        foreach (var eventEntityId in eventQuery)
        {
            _shaderData = world.GetResource<ShaderData>();
            
            var updateEvent = eventQuery.Get<UpdateTerrainMeshEvent>(eventEntityId);

            foreach (var chunkEntityId in chunkQuery)
            {
                var chunkEntity = world.Entity(chunkEntityId);
                var chunkCell = chunkEntity.Get<Vector3i>();

                if (updateEvent.Chunks != null && !updateEvent.Chunks.Contains(chunkCell))
                {
                    continue;
                }

                _terrainMesh = chunkEntity.Get<NodeHandle<TerrainMesh>>().Node;

                ref var locations = ref chunkEntity.Get<Locations>();

                Triangulate(locations);

                var terrainCollider = chunkEntity.Get<NodeHandle<TerrainCollider>>().Node;

                terrainCollider.UpdateCollisionShape(_terrainMesh.Mesh.CreateTrimeshShape());
            }

            _shaderData.Apply();
        }
    }

    private void Triangulate(Locations locations)
    {
        _terrainMesh.Clear();

        foreach (var locEntity in locations.Dict.Values)
        {
            Triangulate(locEntity);

            ref var coords = ref locEntity.Get<Coords>();
            var baseTerrain = locEntity.Get<HasBaseTerrain>().Entity;

            _shaderData.UpdateTerrain((int)coords.Offset.x, (int)coords.Offset.z, baseTerrain.Get<TerrainTypeIndex>().Value);
        }

        _terrainMesh.Apply();
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
        var index = locEntity.Get<Index>().Value;

        ref var elevation = ref locEntity.Get<Elevation>();
        ref var plateauArea = ref locEntity.Get<PlateauArea>();
        ref var terrainEntity = ref locEntity.Get<HasBaseTerrain>().Entity;
        ref var terrainCode = ref terrainEntity.Get<TerrainCode>().Value;

        Vector3 center = locEntity.Get<Coords>().World;
        center.y = elevation.Height;

        EdgeVertices e = new EdgeVertices(
            center + Metrics.GetFirstSolidCorner(direction, plateauArea),
            center + Metrics.GetSecondSolidCorner(direction, plateauArea)
        );

        TriangulatePlateau(center, e, index);

        if (direction <= Direction.W)
        {
            TriangulateConnection(direction, locEntity, e);
        }
    }

    private void TriangulateConnection(Direction direction, EcsEntity locEntity, EdgeVertices e1)
    {
        var locIndex = locEntity.Get<Index>().Value;

        ref var coords = ref locEntity.Get<Coords>();
        ref var elevation = ref locEntity.Get<Elevation>();
        ref var neighbors = ref locEntity.Get<Neighbors>();
        ref var terrainEntity = ref locEntity.Get<HasBaseTerrain>().Entity;

        if (!neighbors.Has(direction))
        {
            return;
        }

        var nLocEntity = neighbors.Get(direction);
        var nLocIndex = nLocEntity.Get<Index>().Value;

        ref var nCoords = ref nLocEntity.Get<Coords>();
        ref var nElevation = ref nLocEntity.Get<Elevation>();
        ref var nPlateauArea = ref nLocEntity.Get<PlateauArea>();
        ref var nTerrainEntity = ref nLocEntity.Get<HasBaseTerrain>().Entity;

        var bridge = Metrics.GetBridge(direction, nPlateauArea);
        bridge.y = (nCoords.World.y + nElevation.Height) - (coords.World.y + elevation.Height);

        var e2 = new EdgeVertices(
            e1.v1 + bridge,
            e1.v5 + bridge
        );

        TriangulateSlope(e1, e2, locIndex, nLocIndex);

        if (direction <= Direction.SW && neighbors.Has(direction.Next()))
        {
            var nextLocEntity = neighbors.Get(direction.Next());
            var nextLocIndex = nextLocEntity.Get<Index>().Value;

            ref var nextElevation = ref nextLocEntity.Get<Elevation>();
            ref var nextPlateauArea = ref nextLocEntity.Get<PlateauArea>();
            ref var nextTerrainEntity = ref nextLocEntity.Get<HasBaseTerrain>().Entity;

            var v6 = e1.v5 + Metrics.GetBridge(direction.Next(), nextPlateauArea);
            v6.y = nextElevation.Height;

            var indices = new Vector3();

            if (elevation.Level <= nElevation.Level)
            {
                if (elevation.Level <= nElevation.Level)
                {
                    indices.x = nLocIndex;
                    indices.y = locIndex;
                    indices.z = nextLocIndex;
                    TriangulateCorner(e1.v5, e2.v5, v6, indices);
                }
                else
                {
                    indices.x = locIndex;
                    indices.y = nextLocIndex;
                    indices.z = nLocIndex;
                    TriangulateCorner(v6, e1.v5, e2.v5, indices);
                }
            }
            else if (nElevation.Level <= nextElevation.Level)
            {
                indices.x = nextLocIndex;
                indices.y = nLocIndex;
                indices.z = locIndex;
                TriangulateCorner(e2.v5, v6, e1.v5, indices);
            }
            else
            {
                indices.x = locIndex;
                indices.y = nextLocIndex;
                indices.z = nLocIndex;
                TriangulateCorner(v6, e1.v5, e2.v5, indices);
            }
        }
    }

    private void TriangulateCorner(Vector3 bottom, Vector3 left, Vector3 right, Vector3 indices)
    {
        // _terrain.AddTrianglePerturbed(bottom, left, right);
        _terrainMesh.AddTriangle(left, bottom, right);
        _terrainMesh.AddTriangleCellData(indices, ColorRed, ColorGreen, ColorBlue);
    }

    private void TriangulatePlateau(Vector3 center, EdgeVertices edge, float index)
    {
        _terrainMesh.AddTriangle(edge.v1, center, edge.v5);
        _terrainMesh.AddTriangleCellData(new Vector3(index, index, index), ColorRed);
        // _terrainMesh.AddTriangle(edge.v1, center, edge.v2);
        // _terrainMesh.AddTriangle(edge.v2, center, edge.v3);
        // _terrainMesh.AddTriangle(edge.v3, center, edge.v4);
        // _terrainMesh.AddTriangle(edge.v4, center, edge.v5);
    }

    void TriangulateSlope(EdgeVertices e1, EdgeVertices e2, float index1, float index2)
    {
        _terrainMesh.AddQuad(e1.v1, e1.v5, e2.v1, e2.v5);
        _terrainMesh.AddQuadCellData(new Vector3(index1, index2, index1), ColorRed, ColorGreen);
        // _terrainMesh.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        // _terrainMesh.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        // _terrainMesh.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        // _terrainMesh.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
    }
}