using Godot;
using Leopotam.Ecs;

public class MapSpawnSystem : IEcsInitSystem
{
    EcsWorld _world;
    Node _parent;

    public MapSpawnSystem(Node parent)
    {
        _parent = parent;
    }

    public void Init()
    {
        GD.Print("New Map Created!");

        var mapEntity = _world.NewEntity();

        mapEntity.Replace(new Grid(50, 50));

        var mapView = new MapView();
        _parent.AddChild(mapView);

        mapEntity.Replace(new ViewHandle<MapView>(mapView));

        ref var locations = ref mapEntity.Get<Locations>();
        ref var grid = ref mapEntity.Get<Grid>();

        for(int z = 0; z < grid.Height; z++)
        {
            for(int x = 0; x < grid.Height; x++)
            {
                var locEntity = _world.NewEntity();

                locEntity.Replace(Coords.FromOffset(x, z));
                
                ref var coords = ref locEntity.Get<Coords>();

                locations.Set(coords.Cube, locEntity);
            }
        }

        var updateMapEventEntity = _world.NewEntity();
        updateMapEventEntity.Get<UpdateMapEvent>();
    }
}