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
            GD.Print("UpdateTerrainFeaturePopulatorEvent Sent!");

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
        if (locEntity.Has<Forest>())
        {
            _terrainFeaturePopulator.AddFeature(locEntity);
        }

        if (locEntity.Has<Castle>())
        {
            _terrainFeaturePopulator.AddCastle(locEntity);
        }
    }
}