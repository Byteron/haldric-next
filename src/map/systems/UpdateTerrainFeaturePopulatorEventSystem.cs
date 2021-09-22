using System.Collections.Generic;
using Godot;
using Leopotam.Ecs;

public struct UpdateTerrainFeaturePopulatorEvent
{
    public List<Vector3i> Chunks;

    public UpdateTerrainFeaturePopulatorEvent(List<Vector3i> chunks = null)
    {
        Chunks = chunks;
    }
}

public class UpdateTerrainFeaturePopulatorEventSystem : IEcsRunSystem
{
    EcsFilter<UpdateTerrainFeaturePopulatorEvent> _events;
    EcsFilter<Locations, NodeHandle<TerrainFeaturePopulator>, NodeHandle<TerrainCollider>> _chunks;

    TerrainFeaturePopulator _terrainFeaturePopulator;

    public void Run()
    {
        foreach (var i in _events)
        {
            foreach (var j in _chunks)
            {
                var eventEntity = _events.GetEntity(i);
                var chunkEntity = _chunks.GetEntity(j);

                var updateEvent = eventEntity.Get<UpdateTerrainFeaturePopulatorEvent>();

                var chunkCell = chunkEntity.Get<Vector3i>();

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