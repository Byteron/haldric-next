using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public struct UpdateTerrainFeaturePopulatorEvent
{
    public List<Vector3i> Chunks { get; set; }

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
        var chunksQuery = commands.Query().Has<Locations, Node<TerrainFeaturePopulator>, Node<TerrainCollider>, Vector3i>();

        commands.Receive((UpdateTerrainFeaturePopulatorEvent e) =>
        {
            foreach (var chunkEntity in chunksQuery)
            {
                ref var chunkCell = ref chunkEntity.Get<Vector3i>();

                if (e.Chunks != null && !e.Chunks.Contains(chunkCell))
                {
                    continue;
                }

                _terrainFeaturePopulator = chunkEntity.Get<Node<TerrainFeaturePopulator>>().Value;

                ref var locations = ref chunkEntity.Get<Locations>();

                Populate(locations);

                var terrainCollider = chunkEntity.Get<Node<TerrainCollider>>().Value;
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
        for (int i = 0; i < 6; i++)
        {
            Populate((Direction)i, locEntity);
        }
    }

    void Populate(Direction direction, Entity locEntity)
    {
        ref var baseTerrain = ref locEntity.Get<HasBaseTerrain>();
        ref var baseTerrainCode = ref baseTerrain.Entity.Get<TerrainCode>();


        Populate(locEntity, baseTerrainCode);

        if (locEntity.Has<HasOverlayTerrain>())
        {
            ref var overlayTerrain = ref locEntity.Get<HasOverlayTerrain>();
            ref var overlayTerrainCode = ref overlayTerrain.Entity.Get<TerrainCode>();

            Populate(locEntity, overlayTerrainCode);
        }
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