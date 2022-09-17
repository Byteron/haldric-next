using System.Runtime.CompilerServices;
using Godot;
using RelEcs;


public static class TerrainPropsExtensions
{
    static TerrainGraphicData Data;

    static Query<Coords, Elevation> Tiles;
    static Query<Entity> RecruitFroms;
    static Query<Entity> RecruitTos;
    static Query<TerrainCode, ElevationOffset> Terrains;

    public static void UpdateTerrainProps(this ISystem system)
    {
        Data = system.GetElement<TerrainGraphicData>();

        Tiles = system.Query<Coords, Elevation>();
        RecruitFroms = system.QueryBuilder<Entity>().Has<CanRecruitFrom>().Build();
        RecruitTos = system.QueryBuilder<Entity>().Has<CanRecruitTo>().Build();
        Terrains = system.Query<TerrainCode, ElevationOffset>();

        var chunkQuery = system.Query<Entity, Chunk, TerrainProps>();
        foreach (var (chunkEntity, chunk, terrainProps) in chunkQuery)
        {
            // if (!chunk.IsDirty) continue;
            // chunk.IsDirty = false;
            
            terrainProps.Clear();
            
            var chunkBaseTerrainTiles = system
                .QueryBuilder<Entity, BaseTerrainSlot>()
                .Has<TileOf>(chunkEntity)
                .Build();

            var chunkOverlayTerrainTiles = system
                .QueryBuilder<Entity, OverlayTerrainSlot>()
                .Has<TileOf>(chunkEntity)
                .Build();

            // Add Decoration

            foreach (var (entity, terrainSlot) in chunkBaseTerrainTiles)
            {
                AddDecoration(terrainProps, entity, terrainSlot.Entity);
            }

            foreach (var (entity, terrainSlot) in chunkOverlayTerrainTiles)
            {
                AddDecoration(terrainProps, entity, terrainSlot.Entity);
            }
            
            terrainProps.Apply();
        }
    }

    static void AddDecoration(TerrainProps props, Entity tileEntity, Entity terrainEntity)
    {
        var (code, offset) = Terrains.Get(terrainEntity);

        if (!Data.Decorations.ContainsKey(code.Value)) return;
        
        var (coords, elevation) = Tiles.Get(tileEntity);

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
}