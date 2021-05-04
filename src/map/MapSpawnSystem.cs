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

        mapEntity.Replace(new Grid(40, 40));

        var mapView = Scenes.Instance.MapView.Instance<MapView>();
        _parent.AddChild(mapView);

        mapEntity.Replace(new ViewHandle<MapView>(mapView));

        ref var locations = ref mapEntity.Get<Locations>();
        ref var grid = ref mapEntity.Get<Grid>();

        InitializeLocations(grid, locations);
        InitializeNeighbors(locations);

        UpdateMap();
    }

    private void InitializeLocations(Grid grid, Locations locations)
    {
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
    }

    private void InitializeNeighbors(Locations locations)
    {
        foreach (var entity in locations.Values)
        {
            ref var coords = ref entity.Get<Coords>();
            ref var neighbors = ref entity.Get<Neighbors>();

            for (Direction direction = Direction.NE; direction <= Direction.SE; direction++)
            {
                Vector3 nCell = Hex.GetNeighbor(coords.Cube, direction);

                if (!locations.Has(nCell))
                {
                    continue;
                }

                var nEntity = locations.Get(nCell);
                neighbors.Set(direction, nEntity);
            }
        }
    }

    private void UpdateMap()
    {
        var updateMapEventEntity = _world.NewEntity();
        updateMapEventEntity.Get<UpdateMapEvent>();
    }
}