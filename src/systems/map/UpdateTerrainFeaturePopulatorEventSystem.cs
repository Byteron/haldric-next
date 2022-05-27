using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public class UpdateTerrainFeaturePopulatorEvent
{
    public List<Vector3i> Chunks { get; }

    public UpdateTerrainFeaturePopulatorEvent(List<Vector3i> chunks = null)
    {
        Chunks = chunks;
    }
}

public class UpdateTerrainFeaturePopulatorEventSystem : ISystem
{
    TerrainFeaturePopulator _terrainFeaturePopulator;

    public void Run(Commands commands)
    {
        var chunksQuery = commands.Query<Locations, TerrainFeaturePopulator, Cell>();

        commands.Receive((UpdateTerrainFeaturePopulatorEvent e) =>
        {
            foreach (var (locations, populator, cell) in chunksQuery)
            {
                if (e.Chunks != null && !e.Chunks.Contains(cell.Value)) continue;
                
                _terrainFeaturePopulator = populator;
                Populate(locations);
            }
        });
    }

    void Populate(Locations locations)
    {
        _terrainFeaturePopulator.Clear();

        foreach (var item in locations.Dict)
        {
            Populate(item.Value);
        }

        _terrainFeaturePopulator.Apply();
    }

    void Populate(Entity locEntity)
    {
        var baseTerrain = locEntity.Get<HasBaseTerrain>();
        var baseTerrainCode = baseTerrain.Entity.Get<TerrainCode>();

        Populate(locEntity, baseTerrainCode);

        if (!locEntity.Has<HasOverlayTerrain>()) return;
        
        var overlayTerrain = locEntity.Get<HasOverlayTerrain>();
        var overlayTerrainCode = overlayTerrain.Entity.Get<TerrainCode>();

        Populate(locEntity, overlayTerrainCode);
    }

    void Populate(Entity locEntity, TerrainCode terrainCode)
    {
        if (Data.Instance.Decorations.ContainsKey(terrainCode.Value))
        {
            _terrainFeaturePopulator.AddDecoration(locEntity, terrainCode.Value);
        }

        if (Data.Instance.DirectionalDecorations.ContainsKey(terrainCode.Value))
        {
            _terrainFeaturePopulator.AddDirectionalDecoration(locEntity, terrainCode.Value);
        }

        if (Data.Instance.WallTowers.ContainsKey(terrainCode.Value))
        {
            _terrainFeaturePopulator.AddTowers(locEntity);
        }

        if (Data.Instance.WallSegments.ContainsKey(terrainCode.Value))
        {
            _terrainFeaturePopulator.AddWalls(locEntity);
        }

        if (Data.Instance.OuterCliffs.ContainsKey(terrainCode.Value))
        {
            _terrainFeaturePopulator.AddOuterCliffs(locEntity);
        }

        if (Data.Instance.InnerCliffs.ContainsKey(terrainCode.Value))
        {
            _terrainFeaturePopulator.AddInnerCliffs(locEntity);
        }

        if (Data.Instance.KeepPlateaus.ContainsKey(terrainCode.Value))
        {
            _terrainFeaturePopulator.AddKeepPlateau(locEntity, terrainCode.Value);
        }

        if (Data.Instance.WaterGraphics.ContainsKey(terrainCode.Value))
        {
            _terrainFeaturePopulator.AddWater(locEntity, terrainCode.Value);
        }
    }
}