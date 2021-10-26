using System.Collections.Generic;
using Godot;
using Bitron.Ecs;

public struct UpdateTerrainFeaturePopulatorEvent
{
    public List<Vector3i> Chunks;

    public UpdateTerrainFeaturePopulatorEvent(List<Vector3i> chunks = null)
    {
        Chunks = chunks;
    }
}

public class UpdateTerrainFeaturePopulatorEventSystem : IEcsSystem
{
    TerrainFeaturePopulator _terrainFeaturePopulator;

    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<UpdateTerrainFeaturePopulatorEvent>().End();
        var chunksQuery = world.Query<Locations>()
            .Inc<NodeHandle<TerrainFeaturePopulator>>()
            .Inc<NodeHandle<TerrainCollider>>()
            .Inc<Vector3i>()
            .End();

        foreach (var eventEntityId in eventQuery)
        {
            foreach (var chunkEntityId in chunksQuery)
            {
                var updateEvent = eventQuery.Get<UpdateTerrainFeaturePopulatorEvent>(eventEntityId);

                var chunkCell = chunksQuery.Get<Vector3i>(chunkEntityId);

                if (updateEvent.Chunks != null && !updateEvent.Chunks.Contains(chunkCell))
                {
                    continue;
                }

                _terrainFeaturePopulator = chunksQuery.Get<NodeHandle<TerrainFeaturePopulator>>(chunkEntityId).Node;

                ref var locations = ref chunksQuery.Get<Locations>(chunkEntityId);

                Populate(locations);

                var terrainCollider = chunksQuery.Get<NodeHandle<TerrainCollider>>(chunkEntityId).Node;
            }
        }
    }

    private void Populate(Locations locations)
    {
        _terrainFeaturePopulator.Clear();

        foreach (var item in locations.Dict)
        {
            Populate(item.Value);
        }

        _terrainFeaturePopulator.Apply();
    }

    private void Populate(EcsEntity locEntity)
    {
        for (Direction direction = Direction.NE; direction <= Direction.SE; direction++)
        {
            Populate(direction, locEntity);
        }
    }

    private void Populate(Direction direction, EcsEntity locEntity)
    {
        ref var baseTerrainEntity = ref locEntity.Get<HasBaseTerrain>().Entity;
        ref var baseTerrainCode = ref baseTerrainEntity.Get<TerrainCode>();


        Populate(locEntity, baseTerrainCode);

        if (locEntity.Has<HasOverlayTerrain>())
        {
            ref var overlayTerrainEntity = ref locEntity.Get<HasOverlayTerrain>().Entity;
            ref var overlayTerrainCode = ref overlayTerrainEntity.Get<TerrainCode>();

            Populate(locEntity, overlayTerrainCode);
        }
    }

    private void Populate(EcsEntity locEntity, TerrainCode terrainCode)
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