using System.Collections.Generic;
using Godot;
using Bitron.Ecs;

public struct UpdateTerrainFeaturePopulatorEvent
{
    public List<Vector3i> Chunks { get; set; }

    public UpdateTerrainFeaturePopulatorEvent(List<Vector3i> chunks = null)
    {
        Chunks = chunks;
    }
}

public class UpdateTerrainFeaturePopulatorEventSystem : IEcsSystem
{
    private TerrainFeaturePopulator _terrainFeaturePopulator;

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
                ref var updateEvent = ref world.Entity(eventEntityId).Get<UpdateTerrainFeaturePopulatorEvent>();

                var chunkEntity = world.Entity(chunkEntityId);

                ref var chunkCell = ref chunkEntity.Get<Vector3i>();

                if (updateEvent.Chunks != null && !updateEvent.Chunks.Contains(chunkCell))
                {
                    continue;
                }

                _terrainFeaturePopulator = chunkEntity.Get<NodeHandle<TerrainFeaturePopulator>>().Node;

                ref var locations = ref chunkEntity.Get<Locations>();

                Populate(locations);

                var terrainCollider = chunkEntity.Get<NodeHandle<TerrainCollider>>().Node;
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
        for (int i = 0; i < 6; i++)
        {
            Populate((Direction)i, locEntity);
        }
    }

    private void Populate(Direction direction, EcsEntity locEntity)
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

        if (Data.Instance.Cliffs.ContainsKey(terrainCode.Value))
        {
            _terrainFeaturePopulator.AddCliffs(locEntity);
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