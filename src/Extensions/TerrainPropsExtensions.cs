using System.Runtime.CompilerServices;
using Godot;
using RelEcs;


public static class TerrainPropsExtensions
{
    static TerrainGraphicData Data;

    static Query<Coords, Elevation, PlateauArea, Neighbors> Tiles;
    static Query<BaseTerrainSlot> BaseTerrainSlots;
    static Query<Entity> RecruitFroms;
    static Query<Entity> RecruitTos;
    static Query<TerrainCode, ElevationOffset> Terrains;

    public static void UpdateTerrainProps(this World world)
    {
        Data = world.GetElement<TerrainGraphicData>();

        Tiles = world.Query<Coords, Elevation, PlateauArea, Neighbors>();
        BaseTerrainSlots = world.Query<BaseTerrainSlot>();
        RecruitFroms = world.QueryBuilder<Entity>().Has<CanRecruitFrom>().Build();
        RecruitTos = world.QueryBuilder<Entity>().Has<CanRecruitTo>().Build();
        Terrains = world.Query<TerrainCode, ElevationOffset>();

        var chunkQuery = world.Query<Entity, Chunk, TerrainProps>();
        foreach (var (chunkEntity, chunk, terrainProps) in chunkQuery)
        {
            // if (!chunk.IsDirty) continue;
            // chunk.IsDirty = false;

            terrainProps.Clear();

            var chunkTiles = world
                .QueryBuilder<Entity, BaseTerrainSlot, OverlayTerrainSlot>()
                .Has<TileOf>(chunkEntity)
                .Build();

            foreach (var (entity, terrainSlot, overlayTerrainSlot) in chunkTiles)
            {
                AddWater(terrainProps, entity, terrainSlot.Entity);
                AddDecoration(terrainProps, entity, terrainSlot.Entity);
                AddDirectionalDecoration(terrainProps, entity, terrainSlot.Entity);
                AddOuterCliffs(terrainProps, entity, terrainSlot.Entity);
                AddInnerCliffs(terrainProps, entity, terrainSlot.Entity);
                AddWalls(terrainProps, entity, terrainSlot.Entity);
                AddTowers(terrainProps, entity, terrainSlot.Entity);

                if (world.IsAlive(overlayTerrainSlot.Entity))
                {
                    AddDecoration(terrainProps, entity, overlayTerrainSlot.Entity);
                    AddDirectionalDecoration(terrainProps, entity, overlayTerrainSlot.Entity);

                }
            }

            terrainProps.Apply();
        }
    }

    static void AddWater(TerrainProps props, Entity tileEntity, Entity terrainEntity)
    {
        var (code, _) = Terrains.Get(terrainEntity);

        if (!Data.WaterGraphics.ContainsKey(code.Value)) return;

        var (coords, elevation, _, _) = Tiles.Get(tileEntity);

        var position = coords.ToWorld();
        position.y = elevation.Height - Metrics.ElevationStep * 0.5f;

        props.AddRenderData(Data.WaterGraphics[code.Value].Mesh, position, Vector3.Zero);
    }

    static void AddDecoration(TerrainProps props, Entity tileEntity, Entity terrainEntity)
    {
        var (code, offset) = Terrains.Get(terrainEntity);

        if (!Data.Decorations.ContainsKey(code.Value)) return;

        var (coords, elevation, _, _) = Tiles.Get(tileEntity);

        var position = coords.ToWorld() + new Vector3(0f, elevation.Height + offset.Value, 0f);

        foreach (var terrainGraphic in Data.Decorations[code.Value].Values)
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

    static void AddDirectionalDecoration(TerrainProps props, Entity tileEntity, Entity terrainEntity)
    {
        var (code, offset) = Terrains.Get(terrainEntity);

        if (!Data.DirectionalDecorations.ContainsKey(code.Value)) return;

        var (coords, elevation, plateauArea, neighbors) = Tiles.Get(tileEntity);

        var center = coords.ToWorld();
        center.y = elevation.Height + offset.Value;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;

            var rotation = direction.Rotation();

            if (!neighbors.Has(direction)) continue;

            var nTileEntity = neighbors.Get(direction);

            var (_, nElevation, _, _) = Tiles.Get(nTileEntity);

            var nTerrainEntity = BaseTerrainSlots.Get(nTileEntity).Entity;
            var (_, nOffset) = Terrains.Get(nTerrainEntity);

            if (elevation.Value != nElevation.Value) continue;

            var elevationOffsetDifference = offset.Value - nOffset.Value;

            if (Mathf.Abs(elevationOffsetDifference) > 0.5f) continue;

            var position = center + Metrics.GetSolidEdgeMiddle(direction, plateauArea.SolidFactor);

            foreach (var terrainGraphic in Data.DirectionalDecorations[code.Value].Values)
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

    public static void AddWalls(TerrainProps props, Entity tileEntity, Entity terrainEntity)
    {
        var (code, offset) = Terrains.Get(terrainEntity);

        if (!Data.WallSegments.ContainsKey(code.Value)) return;

        var (coords, elevation, _, neighbors) = Tiles.Get(tileEntity);

        var center = coords.ToWorld();
        center.y = elevation.Height + offset.Value;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;
            //walls want to rotate the other way it seems?
            var rotation = direction.Rotation();

            if (!neighbors.Has(direction)) continue;

            var nTileEntity = neighbors.Get(direction);

            var (_, nElevation, _, _) = Tiles.Get(nTileEntity);

            var nTerrainEntity = BaseTerrainSlots.Get(nTileEntity).Entity;

            if (nElevation.Value < 0) continue;

            if (elevation.Value == nElevation.Value && RecruitFroms.Has(nTerrainEntity)) continue;

            if (elevation.Value == nElevation.Value
                && !RecruitFroms.Has(terrainEntity)
                && RecruitTos.Has(terrainEntity)
                && RecruitTos.Has(nTerrainEntity)) continue;

            var position = center + Metrics.GetEdgeMiddle(direction);
            props.AddRenderData(Data.WallSegments[code.Value].Mesh, position, new Vector3(0f, rotation, 0f));
        }
    }

    static void AddTowers(TerrainProps props, Entity tileEntity, Entity terrainEntity)
    {
        var (code, offset) = Terrains.Get(terrainEntity);

        if (!Data.WallTowers.ContainsKey(code.Value)) return;

        var (coords, elevation, _, neighbors) = Tiles.Get(tileEntity);

        var center = coords.ToWorld();
        center.y = elevation.Height + offset.Value;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;

            var rotation = direction.Rotation();

            if (!neighbors.Has(direction)) continue;

            var nTileEntity = neighbors.Get(direction);

            var (_, nElevation, _, _) = Tiles.Get(nTileEntity);

            var nTerrainEntity = BaseTerrainSlots.Get(nTileEntity).Entity;

            if (nElevation.Value < 0) continue;
            if (elevation.Value == nElevation.Value && RecruitFroms.Has(nTerrainEntity)) continue;
            if (elevation.Value == nElevation.Value) continue;

            if (!RecruitFroms.Has(terrainEntity)
                && RecruitTos.Has(terrainEntity)
                && RecruitTos.Has(nTerrainEntity)) continue;

            var position = center + Metrics.GetFirstCorner(direction);
            props.AddRenderData(Data.WallTowers[code.Value].Mesh, position, new Vector3(0f, rotation, 0f));
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

    static void AddOuterCliffs(TerrainProps props, Entity tileEntity, Entity terrainEntity)
    {
        var (code, offset) = Terrains.Get(terrainEntity);

        if (!Data.OuterCliffs.ContainsKey(code.Value)) return;

        var (coords, elevation, _, neighbors) = Tiles.Get(tileEntity);

        var center = coords.ToWorld();
        center.y = elevation.Height + offset.Value;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;

            // cliffs want to rotate the other way it seems?
            var rotation = direction.Rotation();

            if (!neighbors.Has(direction)) continue;

            var nTileEntity = neighbors.Get(direction);

            var (_, nElevation, _, _) = Tiles.Get(nTileEntity);

            var nTerrainEntity = BaseTerrainSlots.Get(nTileEntity).Entity;
            var (nCode, _) = Terrains.Get(nTerrainEntity);

            if (Data.InnerCliffs.ContainsKey(nCode.Value)) continue;

            var elevationDiff = elevation.Value - nElevation.Value;

            if (elevationDiff < 2) continue;

            var position = center + Metrics.GetEdgeMiddle(direction);

            foreach (var terrainGraphic in Data.OuterCliffs[code.Value].Values)
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

    static void AddInnerCliffs(TerrainProps props, Entity tileEntity, Entity terrainEntity)
    {
        var (code, offset) = Terrains.Get(terrainEntity);

        if (!Data.InnerCliffs.ContainsKey(code.Value)) return;

        var (coords, elevation, _, neighbors) = Tiles.Get(tileEntity);

        var center = coords.ToWorld();
        center.y = elevation.Height + offset.Value;

        for (var i = 0; i < 6; i++)
        {
            var direction = (Direction)i;

            // cliffs want to rotate the other way it seems?
            var rotation = direction.Opposite().Rotation();

            if (!neighbors.Has(direction)) continue;

            var nTileEntity = neighbors.Get(direction);

            var (_, nElevation, _, _) = Tiles.Get(nTileEntity);

            var nTerrainEntity = BaseTerrainSlots.Get(nTileEntity).Entity;
            var (_, nOffset) = Terrains.Get(nTerrainEntity);

            var elevationDiff = nElevation.Value - elevation.Value;

            if (elevationDiff < 2) continue;

            center.y = nElevation.Height + nOffset.Value;

            var position = center + Metrics.GetEdgeMiddle(direction);

            foreach (var terrainGraphic in Data.InnerCliffs[code.Value].Values)
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
}