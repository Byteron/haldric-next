using RelEcs;
using Godot;

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
}

public static class TerrainMeshSystems
{
    static readonly Color ColorRed = new(1.0f, 0f, 0f);
    static readonly Color ColorGreen = new(0f, 1.0f, 0f);
    static readonly Color ColorBlue = new(0f, 0f, 1.0f);

    public static void UpdateTerrainGraphics(World world)
    {
        UpdateTerrainMesh(world);
        UpdateTerrainProps(world);
    }

    public static void UpdateTerrainMesh(World world)
    {
        var shaderData = world.GetElement<ShaderData>();

        var terrainTypeIndices = world.Query<TerrainTypeIndex>();
        var elevationOffsets = world.Query<ElevationOffset>();

        var chunkQuery = world.Query<Entity, Chunk, TerrainMesh, TerrainCollider>();
        foreach (var (chunkEntity, chunk, mesh, collider) in chunkQuery)
        {
            if (!chunk.IsDirty) continue;
            chunk.IsDirty = false;

            mesh.Clear();

            // Update Terrain Mesh

            var allTiles = world.Query<Index, Coords, BaseTerrainSlot, Elevation, PlateauArea>();
            var chunkTiles = world
                .QueryBuilder<Index, Coords, BaseTerrainSlot, Neighbors, Elevation, PlateauArea>()
                .Has<TileOf>(chunkEntity)
                .Build();

            foreach (var (index, coords, baseTerrainSlot, neighbors, elevation, plateauArea) in chunkTiles)
            {
                var elevationOffset = elevationOffsets.Get(baseTerrainSlot.Entity);
                
                var center = coords.ToWorld();
                center.Y = elevation.Height + elevationOffset.Value;
                
                for (var direction = Direction.E; direction <= Direction.NE; direction++)
                {
                    // Triangulate Plateau - this is OK
                    
                    var e1 = new EdgeVertices(
                        center + Metrics.GetFirstSolidCorner(direction, plateauArea.SolidFactor),
                        center + Metrics.GetSecondSolidCorner(direction, plateauArea.SolidFactor)
                    );

                    mesh.TriangulatePlateau(center, e1, index.Value);

                    if (direction <= Direction.SW)
                    {
                        // Triangulate Slope - this is already BOTCHED

                        if (!neighbors.Has(direction)) continue;
                        
                        var nTileEntity = neighbors.Get(direction);
                        var (nIndex, nCoords, nBaseTerrainSlot, nElevation, nPlateauArea) = allTiles.Get(nTileEntity);

                        var nElevationOffset = elevationOffsets.Get(nBaseTerrainSlot.Entity);
                        
                        var nCenter = nCoords.ToWorld();
                        nCenter.Y = nElevation.Height + nElevationOffset.Value;
                        
                        var bridge = Metrics.GetBridge(direction, nPlateauArea.BlendFactor);
                        bridge.Y = nCenter.Y - center.Y;

                        var e2 = new EdgeVertices(
                            e1.V1 + bridge,
                            e1.V5 + bridge
                        );

                        mesh.TriangulateSlope(e1, e2, index.Value, nIndex.Value);
                        
                        // Triangulate Corners - also definitely BOTCHED
                        
                        if (!neighbors.Has(direction.Next())) continue;
                        
                        var nextTileEntity = neighbors.Get(direction.Next());
                        var (nextIndex, nextCoords, nextBaseTerrainSlot, nextElevation, nextPlateauArea) = allTiles.Get(nextTileEntity);
                        
                        var nextElevationOffset = elevationOffsets.Get(nextBaseTerrainSlot.Entity);
                        
                        var nextCenter = nextCoords.ToWorld();
                        nextCenter.Y = nextElevation.Height + nextElevationOffset.Value;

                        var v6 = e1.V5 + Metrics.GetBridge(direction.Next(), nextPlateauArea.BlendFactor);
                        v6.Y = nextCenter.Y;
                        
                        var indices = new Vector3();
                        
                        if (elevation.Value <= nElevation.Value)
                        {
                            indices.X = index.Value;
                            indices.Y = nIndex.Value;
                            indices.Z = nextIndex.Value;
                            mesh.TriangulateCorner(e1.V5, e2.V5, v6, indices);
                        }
                        else if (nElevation.Value <= nextElevation.Value)
                        {
                            indices.X = nIndex.Value;
                            indices.Y = nextIndex.Value;
                            indices.Z = index.Value;
                            mesh.TriangulateCorner(e2.V5, v6, e1.V5, indices);
                        }
                        else
                        {
                            indices.X = nextIndex.Value;
                            indices.Y = index.Value;
                            indices.Z = nIndex.Value;
                            mesh.TriangulateCorner(v6, e1.V5, e2.V5, indices);
                        }
                    }
                }
            }

            mesh.Apply();
            collider.UpdateCollisionShape(mesh.Mesh.CreateTrimeshShape());
        }

        // Update Shader Data

        foreach (var (coords, baseTerrainSlot) in world.Query<Coords, BaseTerrainSlot>())
        {
            var offset = coords.ToOffset();

            var x = (int)offset.X;
            var z = (int)offset.Z;
            var index = terrainTypeIndices.Get(baseTerrainSlot.Entity).Value;

            shaderData.UpdateTerrain(x, z, index);
        }

        shaderData.Apply();
    }

    static void TriangulateCorner(this TerrainMesh mesh, Vector3 bottom, Vector3 left, Vector3 right, Vector3 indices)
    {
        mesh.AddTrianglePerturbed(bottom, left, right);
        mesh.AddTriangleCellData(indices, ColorRed, ColorGreen, ColorBlue);
    }

    static void TriangulatePlateau(this TerrainMesh mesh, Vector3 center, EdgeVertices edge, float index)
    {
        // _terrainMesh.AddTrianglePerturbed(edge.v1, center, edge.v5);
        mesh.AddTrianglePerturbed(edge.V2, center, edge.V1);
        mesh.AddTrianglePerturbed(edge.V3, center, edge.V2);
        mesh.AddTrianglePerturbed(edge.V4, center, edge.V3);
        mesh.AddTrianglePerturbed(edge.V5, center, edge.V4);
        mesh.AddTriangleCellData(new Vector3(index, index, index), ColorRed);
        mesh.AddTriangleCellData(new Vector3(index, index, index), ColorRed);
        mesh.AddTriangleCellData(new Vector3(index, index, index), ColorRed);
        mesh.AddTriangleCellData(new Vector3(index, index, index), ColorRed);
    }

    static void TriangulateSlope(this TerrainMesh mesh, EdgeVertices e1, EdgeVertices e2, float index1, float index2)
    {
        // _terrainMesh.AddQuadPerturbed(e1.v1, e1.v5, e2.v1, e2.v5);
        mesh.AddQuadPerturbed(e1.V2, e1.V1, e2.V2, e2.V1);
        mesh.AddQuadPerturbed(e1.V3, e1.V2, e2.V3, e2.V2);
        mesh.AddQuadPerturbed(e1.V4, e1.V3, e2.V4, e2.V3);
        mesh.AddQuadPerturbed(e1.V5, e1.V4, e2.V5, e2.V4);
        mesh.AddQuadCellData(new Vector3(index1, index2, index1), ColorRed, ColorGreen);
        mesh.AddQuadCellData(new Vector3(index1, index2, index1), ColorRed, ColorGreen);
        mesh.AddQuadCellData(new Vector3(index1, index2, index1), ColorRed, ColorGreen);
        mesh.AddQuadCellData(new Vector3(index1, index2, index1), ColorRed, ColorGreen);
    }
}