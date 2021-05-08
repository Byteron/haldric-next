using Godot;
using Leopotam.Ecs;

struct Forest { }
struct Castle { }

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
        var mapEntity = _world.NewEntity();

        mapEntity.Replace(new Grid(20, 20));

        var terrainMesh = new TerrainMesh();
        var terrainCollider = new TerrainCollider();
        var terrainFeaturePopulator = new TerrainFeaturePopulator();
        
        _parent.AddChild(terrainMesh);
        _parent.AddChild(terrainCollider);
        _parent.AddChild(terrainFeaturePopulator);
        
        mapEntity.Replace(new NodeHandle<TerrainCollider>(terrainCollider));
        mapEntity.Replace(new NodeHandle<TerrainFeaturePopulator>(terrainFeaturePopulator));
        mapEntity.Replace(new NodeHandle<TerrainMesh>(terrainMesh));

        ref var locations = ref mapEntity.Get<Locations>();
        ref var grid = ref mapEntity.Get<Grid>();

        InitializeHoveredCoords();
        InitializeLocations(grid, locations);
        InitializeNeighbors(locations);

        SendUpdateMapEvent();
    }

    private void InitializeHoveredCoords()
    {
        var hoveredCoordsEntity = _world.NewEntity();
        hoveredCoordsEntity.Get<HoveredCoords>();
    }

    private void InitializeLocations(Grid grid, Locations locations)
    {
        for (int z = 0; z < grid.Height; z++)
        {
            for (int x = 0; x < grid.Height; x++)
            {
                var locEntity = _world.NewEntity();

                locEntity.Replace(Coords.FromOffset(x, z));
                locEntity.Replace(new HasTerrain(Data.Instance.Terrains["Gg"].Copy()));
                locEntity.Replace(new Elevation(0));
                locEntity.Replace(new PlateauArea(0.75f));

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

    private void SendUpdateMapEvent()
    {
        var updateMapEventEntity = _world.NewEntity();
        updateMapEventEntity.Get<UpdateMapEvent>();
    }
}