using System.Collections.Generic;
using Godot;

namespace Haldric;

public partial class Chunks : Node3D
{
    static readonly Color ColorRed = new(1.0f, 0f, 0f);
    static readonly Color ColorGreen = new(0f, 1.0f, 0f);
    static readonly Color ColorBlue = new(0f, 0f, 1.0f);

    readonly Dictionary<Vector2I, Chunk> _chunks = new();
    TerrainShader _shader = default!;

    public void Initialize(Grid grid, Dictionary<Coords, Tile> tiles)
    {
        _shader = new TerrainShader(grid.Width, grid.Height);

        var chunkSize = new Vector2I(4, 4);

        for (var z = 0; z < grid.Height; z++)
        {
            for (var x = 0; x < grid.Height; x++)
            {
                var coords = Coords.FromOffset(x, z);

                var chunkVector = coords.ToOffset() / new Vector3(chunkSize.X, 0f, chunkSize.Y);
                var chunkCell = new Vector2I((int)chunkVector.X, (int)chunkVector.Z);

                if (!_chunks.ContainsKey(chunkCell))
                {
                    var terrainMesh = new TerrainMesh();
                    var terrainCollider = new TerrainCollider();
                    var terrainProps = new TerrainProps();

                    AddChild(terrainMesh);
                    AddChild(terrainCollider);
                    AddChild(terrainProps);

                    var chunk = new Chunk
                    {
                        Cell = chunkCell,
                        Mesh = terrainMesh,
                        Collider = terrainCollider,
                        Props = terrainProps
                    };

                    _chunks.Add(chunkCell, chunk);
                }

                tiles[coords].ChunkCell = chunkCell;
            }
        }
    }

    public void UpdateTerrainMeshes(Dictionary<Coords, Tile> tiles)
    {
        foreach (var chunk in _chunks.Values)
        {
            chunk.Mesh.Clear();
        }

        foreach (var tile in tiles.Values)
        {
            var chunk = _chunks[tile.ChunkCell];
            var mesh = chunk.Mesh;

            var center = tile.WorldPosition;
            center.Y += tile.BaseTerrain.ElevationOffset;

            for (var direction = Direction.E; direction <= Direction.Ne; direction++)
            {
                var e1 = new EdgeVertices(
                    center + Metrics.GetFirstSolidCorner(direction, tile.SolidFactor),
                    center + Metrics.GetSecondSolidCorner(direction, tile.SolidFactor)
                );

                TriangulatePlateau(mesh, center, e1, tile.Index);

                if (direction > Direction.Sw) continue;

                var nTile = tile.Neighbors.Get(direction);
                if (nTile is null) continue;

                var nCenter = nTile.WorldPosition;
                nCenter.Y += nTile.BaseTerrain.ElevationOffset;

                var bridge = Metrics.GetBridge(direction, nTile.BlendFactor);
                bridge.Y = nCenter.Y - center.Y;

                var e2 = new EdgeVertices(
                    e1.V1 + bridge,
                    e1.V5 + bridge
                );

                TriangulateSlope(mesh, e1, e2, tile.Index, nTile.Index);


                var nextTile = tile.Neighbors.Get(direction.Next());
                if (nextTile is null) continue;

                var nextCenter = nextTile.WorldPosition;
                nextCenter.Y += nextTile.BaseTerrain.ElevationOffset;

                var v6 = e1.V5 + Metrics.GetBridge(direction.Next(), nextTile.BlendFactor);
                v6.Y = nextCenter.Y;

                var indices = new Vector3();

                if (tile.Elevation <= nTile.Elevation)
                {
                    indices.X = tile.Index;
                    indices.Y = nTile.Index;
                    indices.Z = nextTile.Index;
                    TriangulateCorner(mesh, e1.V5, e2.V5, v6, indices);
                }
                else if (nTile.Elevation <= nextTile.Elevation)
                {
                    indices.X = nTile.Index;
                    indices.Y = nextTile.Index;
                    indices.Z = tile.Index;
                    TriangulateCorner(mesh, e2.V5, v6, e1.V5, indices);
                }
                else
                {
                    indices.X = nextTile.Index;
                    indices.Y = tile.Index;
                    indices.Z = nTile.Index;
                    TriangulateCorner(mesh, v6, e1.V5, e2.V5, indices);
                }
            }
        }

        foreach (var chunk in _chunks.Values)
        {
            chunk.Mesh.Apply();
            chunk.Collider.CollisionShape.Shape = chunk.Mesh.Mesh.CreateTrimeshShape();
        }

        foreach (var tile in tiles.Values)
        {
            var offset = tile.Coords.ToOffset();

            var x = (int)offset.X;
            var z = (int)offset.Z;
            var index = tile.BaseTerrain.Index;
            _shader.UpdateTerrain(x, z, index);
        }

        _shader.UpdateTerrain(0, 0, 3);
        _shader.UpdateTerrain(1, 0, 5);

        _shader.Apply();
    }

    public void UpdateTerrainProps(Dictionary<Coords, Tile> tiles)
    {
        foreach (var chunk in _chunks.Values)
        {
            chunk.Props.Clear();
        }

        foreach (var tile in tiles.Values)
        {
            var chunk = _chunks[tile.ChunkCell];
            var props = chunk.Props;

            AddWater(props, tile, tile.BaseTerrain);
            AddDecoration(props, tile, tile.BaseTerrain);
            AddDirectionalDecoration(props, tile, tile.BaseTerrain);
            AddOuterCliffs(props, tile, tile.BaseTerrain);
            AddInnerCliffs(props, tile, tile.BaseTerrain);
            AddWalls(props, tile, tile.BaseTerrain);
            AddTowers(props, tile, tile.BaseTerrain);

            if (tile.OverlayTerrain is null) continue;

            AddDecoration(props, tile, tile.OverlayTerrain);
            AddDirectionalDecoration(props, tile, tile.OverlayTerrain);
        }

        foreach (var chunk in _chunks.Values)
        {
            chunk.Props.Apply();
        }
    }

    static void AddWater(TerrainProps props, Tile tile, Terrain terrain)
    {
        var code = terrain.Code;

        if (!Data.Instance.WaterGraphics.ContainsKey(code)) return;

        var position = tile.WorldPosition;
        position.Y -= Metrics.ElevationStep * 0.5f;

        props.AddRenderData(Data.Instance.WaterGraphics[code].Mesh, position, Vector3.Zero);
    }

    static void AddDecoration(TerrainProps props, Tile tile, Terrain terrain)
    {
        var code = terrain.Code;
        var offset = terrain.ElevationOffset;

        if (!Data.Instance.Decorations.ContainsKey(code)) return;

        var position = tile.WorldPosition;
        position.Y += offset;

        foreach (var terrainGraphic in Data.Instance.Decorations[code].Values)
        {
            if (terrainGraphic.Variations.Count == 0)
            {
                props.AddRenderData(terrainGraphic.Mesh, position, Vector3.Zero);
            }
            else
            {
                if (!props.RandomIndices.TryGetValue(position, out var index))
                {
                    index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                    props.RandomIndices.Add(position, index);
                }

                if (terrainGraphic.Variations.Count <= index)
                {
                    props.RandomIndices.Remove(position);
                    index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                    props.RandomIndices.Add(position, index);
                }

                var mesh = terrainGraphic.Variations[index];
                props.AddRenderData(mesh, position, Vector3.Zero);
            }
        }
    }

    static void AddDirectionalDecoration(TerrainProps props, Tile tile, Terrain terrain)
    {
        var code = terrain.Code;
        var offset = terrain.ElevationOffset;

        if (!Data.Instance.DirectionalDecorations.ContainsKey(code)) return;

        var elevation = tile.Elevation;
        var solidFactor = tile.SolidFactor;
        var neighbors = tile.Neighbors;

        var center = tile.WorldPosition;
        center.Y += offset;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;

            var rotation = direction.Rotation();


            var nTile = neighbors.Get(direction);
            if (nTile is null) continue;

            var nElevation = nTile.Elevation;

            var nTerrain = tile.BaseTerrain;
            var nOffset = nTerrain.ElevationOffset;

            if (elevation != nElevation) continue;

            var elevationOffsetDifference = offset - nOffset;

            if (Mathf.Abs(elevationOffsetDifference) > 0.5f) continue;

            var position = center + Metrics.GetSolidEdgeMiddle(direction, solidFactor);

            foreach (var terrainGraphic in Data.Instance.DirectionalDecorations[code].Values)
            {
                if (terrainGraphic.Variations.Count == 0)
                {
                    props.AddRenderData(terrainGraphic.Mesh, position, new Vector3(0f, rotation, 0f));
                }
                else
                {
                    if (!props.RandomIndices.TryGetValue(position, out var index))
                    {
                        index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                        props.RandomIndices.Add(position, index);
                    }

                    if (terrainGraphic.Variations.Count <= index)
                    {
                        props.RandomIndices.Remove(position);
                        index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                        props.RandomIndices.Add(position, index);
                    }

                    var mesh = terrainGraphic.Variations[index];
                    props.AddRenderData(mesh, position, new Vector3(0f, rotation, 0f));
                }
            }
            // AddRenderData(Data.Instance.DirectionalDecorations[terrainCode].Mesh, position, new Vector3(0f, rotation, 0f));
        }
    }

    public static void AddWalls(TerrainProps props, Tile tile, Terrain terrain)
    {
        var code = terrain.Code;
        var offset = terrain.ElevationOffset;

        if (!Data.Instance.WallSegments.ContainsKey(code)) return;

        var coords = tile.Coords;
        var elevation = tile.Elevation;
        var neighbors = tile.Neighbors;

        var center = tile.WorldPosition;
        center.Y += offset;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;
            var rotation = direction.Rotation();

            var nTile = neighbors.Get(direction);
            if (nTile is null) continue;

            var nElevation = nTile.Elevation;
            var nTerrain = nTile.BaseTerrain;

            if (nElevation < 0) continue;

            if (elevation == nElevation && nTerrain.CanRecruitFrom) continue;

            if (elevation == nElevation
                && terrain is { CanRecruitFrom: false, CanRecruitTo: true }
                && nTerrain.CanRecruitTo)
            {
                continue;
            }

            var position = center + Metrics.GetEdgeMiddle(direction);
            props.AddRenderData(Data.Instance.WallSegments[code].Mesh, position, new Vector3(0f, rotation, 0f));
        }
    }

    static void AddTowers(TerrainProps props, Tile tile, Terrain terrain)
    {
        var code = terrain.Code;
        var offset = terrain.ElevationOffset;

        if (!Data.Instance.WallTowers.ContainsKey(code)) return;

        var coords = tile.Coords;
        var elevation = tile.Elevation;
        var neighbors = tile.Neighbors;

        var center = tile.WorldPosition;
        center.Y += offset;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;
            var rotation = direction.Rotation();

            var nTile = neighbors.Get(direction);
            if (nTile is null) continue;

            var nElevation = nTile.Elevation;
            var nTerrain = nTile.BaseTerrain;

            if (nElevation < 0) continue;
            if (elevation == nElevation && nTerrain.CanRecruitFrom) continue;
            if (elevation == nElevation) continue;

            if (terrain is { CanRecruitFrom: false, CanRecruitTo: true } && nTerrain.CanRecruitTo) continue;

            var position = center + Metrics.GetFirstCorner(direction);
            props.AddRenderData(Data.Instance.WallTowers[code].Mesh, position, new Vector3(0f, rotation, 0f));
        }
    }

    // TODO: copied from TerrainProps.cs, needs fixing
    // public void AddKeepPlateau(Entity locEntity, string terrainCode)
    // {
    //     var coords = locEntity.Get<Coords>();
    //     var elevation = locEntity.Get<Elevation>();
    //
    //     var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
    //     var elevationOffset = terrainEntity.Get<ElevationOffset>();
    //     var position = coords.World();
    //
    //     position.y = elevation.Height + elevationOffset.Value;
    //     position += Data.Instance.KeepPlateaus[terrainCode].Offset;
    //
    //     AddRenderData(Data.Instance.KeepPlateaus[terrainCode].Mesh, position, Vector3.Zero);
    // }

    static void AddOuterCliffs(TerrainProps props, Tile tile, Terrain terrain)
    {
        var code = terrain.Code;
        var offset = terrain.ElevationOffset;

        if (!Data.Instance.OuterCliffs.ContainsKey(code)) return;

        var coords = tile.Coords;
        var elevation = tile.Elevation;
        var neighbors = tile.Neighbors;

        var center = tile.WorldPosition;
        center.Y += offset;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;

            // cliffs want to rotate the other way it seems?
            var rotation = direction.Rotation();


            var nTile = neighbors.Get(direction);
            if (nTile is null) continue;

            var nElevation = nTile.Elevation;

            var nTerrain = nTile.BaseTerrain;
            var nCode = nTerrain.Code;

            if (Data.Instance.InnerCliffs.ContainsKey(nCode)) continue;

            var elevationDiff = elevation - nElevation;

            if (elevationDiff < 2) continue;

            var position = center + Metrics.GetEdgeMiddle(direction);

            foreach (var terrainGraphic in Data.Instance.OuterCliffs[code].Values)
            {
                if (terrainGraphic.Variations.Count == 0)
                {
                    props.AddRenderData(terrainGraphic.Mesh, position, new Vector3(0f, rotation, 0f));
                }
                else
                {
                    if (!props.RandomIndices.TryGetValue(position, out var index))
                    {
                        index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                        props.RandomIndices.Add(position, index);
                    }

                    if (terrainGraphic.Variations.Count <= index)
                    {
                        props.RandomIndices.Remove(position);
                        index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                        props.RandomIndices.Add(position, index);
                    }

                    var mesh = terrainGraphic.Variations[index];
                    props.AddRenderData(mesh, position, new Vector3(0f, rotation, 0f));
                }
            }
        }
    }

    static void AddInnerCliffs(TerrainProps props, Tile tile, Terrain terrain)
    {
        var code = terrain.Code;
        var offset = terrain.ElevationOffset;

        if (!Data.Instance.InnerCliffs.ContainsKey(code)) return;

        var coords = tile.Coords;
        var elevation = tile.Elevation;
        var neighbors = tile.Neighbors;

        var center = tile.WorldPosition;
        center.Y += offset;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;

            // cliffs want to rotate the other way it seems?
            var rotation = direction.Opposite().Rotation();


            var nTile = neighbors.Get(direction);
            if (nTile is null) continue;

            var nElevation = nTile.Elevation;

            var nTerrain = nTile.BaseTerrain;
            var nOffset = nTerrain.ElevationOffset;

            var elevationDiff = nElevation - elevation;

            if (elevationDiff < 2) continue;

            center.Y = nElevation * Metrics.ElevationStep + nOffset;

            var position = center + Metrics.GetEdgeMiddle(direction);

            foreach (var terrainGraphic in Data.Instance.InnerCliffs[code].Values)
            {
                if (terrainGraphic.Variations.Count == 0)
                {
                    props.AddRenderData(terrainGraphic.Mesh, position, new Vector3(0f, rotation, 0f));
                }
                else
                {
                    if (!props.RandomIndices.TryGetValue(position, out var index))
                    {
                        index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                        props.RandomIndices.Add(position, index);
                    }

                    if (terrainGraphic.Variations.Count <= index)
                    {
                        props.RandomIndices.Remove(position);
                        index = (int)(GD.Randi() % terrainGraphic.Variations.Count);
                        props.RandomIndices.Add(position, index);
                    }

                    var mesh = terrainGraphic.Variations[index];
                    props.AddRenderData(mesh, position, new Vector3(0f, rotation, 0f));
                }
            }
        }
    }

    static void TriangulateCorner(TerrainMesh mesh, Vector3 bottom, Vector3 left, Vector3 right, Vector3 indices)
    {
        mesh.AddTrianglePerturbed(bottom, left, right);
        mesh.AddTriangleCellData(indices, ColorRed, ColorGreen, ColorBlue);
    }

    static void TriangulatePlateau(TerrainMesh mesh, Vector3 center, EdgeVertices edge, float index)
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

    static void TriangulateSlope(TerrainMesh mesh, EdgeVertices e1, EdgeVertices e2, float index1, float index2)
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

public struct EdgeVertices
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