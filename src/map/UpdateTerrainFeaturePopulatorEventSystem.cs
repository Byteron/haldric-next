using Godot;
using Leopotam.Ecs;

public struct UpdateTerrainFeaturePopulatorEvent {}

public class UpdateTerrainFeaturePopulatorEventSystem : IEcsRunSystem
{
    EcsFilter<UpdateTerrainFeaturePopulatorEvent> _events;
    EcsFilter<Locations, NodeHandle<TerrainFeaturePopulator>> _maps;

    TerrainFeaturePopulator _terrainFeaturePopulator;

    public void Run()
    {
        foreach (var i in _events)
        {
            foreach (var j in _maps)
            {
                var mapEntity = _maps.GetEntity(j);

                _terrainFeaturePopulator = mapEntity.Get<NodeHandle<TerrainFeaturePopulator>>().Node;
                
                ref var locations = ref mapEntity.Get<Locations>();
                
                Populate(locations);

                var terrainCollider = mapEntity.Get<NodeHandle<TerrainCollider>>().Node;
            }

            _events.GetEntity(i).Destroy();
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
        ref var terrain = ref locEntity.Get<Terrain>();

        if (Data.Instance.Decorations.ContainsKey(terrain.Code))
        {
            _terrainFeaturePopulator.AddDecoration(locEntity);
        }

        if (Data.Instance.WallTowers.ContainsKey(terrain.Code))
        {
            _terrainFeaturePopulator.AddTowers(locEntity);
        }

        if (Data.Instance.WallSegments.ContainsKey(terrain.Code))
        {
            _terrainFeaturePopulator.AddWalls(locEntity);
        }

        if (Data.Instance.KeepPlateaus.ContainsKey(terrain.Code))
        {
            _terrainFeaturePopulator.AddKeepPlateau(locEntity);
        }
    }
}