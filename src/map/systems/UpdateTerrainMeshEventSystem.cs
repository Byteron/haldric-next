using System.Collections.Generic;
using Godot;
using Leopotam.Ecs;

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

public class UpdateTerrainMeshEventSystem : IEcsRunSystem
{
    EcsFilter<UpdateTerrainMeshEvent> _events;
    EcsFilter<Locations, NodeHandle<TerrainMesh>, NodeHandle<TerrainCollider>> _chunks;

    TerrainMesh _terrainMesh;

    public void Run()
    {
        foreach (var i in _events)
        {
            foreach (var j in _chunks)
            {
                var eventEntity = _events.GetEntity(i);
                var chunkEntity = _chunks.GetEntity(j);

                var updateEvent = eventEntity.Get<UpdateTerrainMeshEvent>();

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

            _events.GetEntity(i).Destroy();
        }
    }

    private void Triangulate(Locations locations)
    {
        _terrainMesh.Clear();

        foreach (var item in locations.Dict)
        {
            Triangulate(item.Value);
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
        // _terrain.AddTrianglePerturbed(left, bottom, right);
        _terrainMesh.AddTriangle(left, bottom, right);
    }

    private void TriangulatePlateau(Vector3 center, EdgeVertices edge)
    {
        _terrainMesh.AddTriangle(edge.v1, center, edge.v5);

        // _terrainMesh.AddTriangle(edge.v1, center, edge.v2);
        // _terrainMesh.AddTriangle(edge.v2, center, edge.v3);
        // _terrainMesh.AddTriangle(edge.v3, center, edge.v4);
        // _terrainMesh.AddTriangle(edge.v4, center, edge.v5);
    }

    void TriangulateSlope(EdgeVertices e1, EdgeVertices e2)
    {
        _terrainMesh.AddQuad(e1.v1, e1.v5, e2.v1, e2.v5);

        // _terrainMesh.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        // _terrainMesh.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        // _terrainMesh.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        // _terrainMesh.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
    }
}