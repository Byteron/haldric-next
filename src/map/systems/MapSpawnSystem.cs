using Godot;
using Leopotam.Ecs;

public struct Chunk { }
public struct Map { }

public struct ChunkSize
{
    public int X;
    public int Z;

    public ChunkSize(int x, int z)
    {
        X = x;
        Z = z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(X, 0f, Z);
    }
}

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

        mapEntity.Replace(new Grid(40, 40));
        mapEntity.Replace(new ChunkSize(5, 5));

        mapEntity.Get<Map>();

        ref var locations = ref mapEntity.Get<Locations>();
        ref var grid = ref mapEntity.Get<Grid>();
        ref var chunkSize = ref mapEntity.Get<ChunkSize>();
        ref var chunks = ref mapEntity.Get<Chunks>();

        InitializeHoveredCoords();
        InitializeLocations(grid, locations);
        InitializeNeighbors(locations);
        InitializeChunks(chunks, chunkSize, grid, locations);

        SendUpdateMapEvent();
    }

    private void InitializeHoveredCoords()
    {
        var hoveredCoordsEntity = _world.NewEntity();
        hoveredCoordsEntity.Get<HoveredCoords>();
    }

    private void InitializeChunks(Chunks chunks, ChunkSize chunkSize, Grid grid, Locations locations)
    {
        for (int z = 0; z < grid.Height; z++)
        {
            for (int x = 0; x < grid.Height; x++)
            {
                var coords = Coords.FromOffset(x, z);
                var chunkCell = (coords.Offset / chunkSize.ToVector3());
                var chunkCelli = new Vector3i((int)chunkCell.x, 0, (int)chunkCell.z);

                if (!chunks.Has(chunkCelli))
                {
                    chunks.Set(chunkCelli, _world.NewEntity());
                }

                var chunkEntity = chunks.Get(chunkCelli);
                ref var chunkLocations = ref chunkEntity.Get<Locations>();
                var locEntity = locations.Get(coords.Cube);

                chunkLocations.Set(coords.Cube, locEntity);

                chunkEntity.Replace(chunkCelli);
                locEntity.Replace(chunkCelli);
            }
        }

        var chunkCount = 0;
        foreach (var chunkEntity in chunks.Values)
        {
            var terrainMesh = new TerrainMesh();
            var terrainCollider = new TerrainCollider();
            var terrainFeaturePopulator = new TerrainFeaturePopulator();

            _parent.AddChild(terrainMesh);
            _parent.AddChild(terrainCollider);
            _parent.AddChild(terrainFeaturePopulator);

            chunkEntity.Get<Chunk>();
            chunkEntity.Replace(new NodeHandle<TerrainCollider>(terrainCollider));
            chunkEntity.Replace(new NodeHandle<TerrainFeaturePopulator>(terrainFeaturePopulator));
            chunkEntity.Replace(new NodeHandle<TerrainMesh>(terrainMesh));

            chunkCount += 1;
        }

        GD.Print(chunkCount, " Chunks Created");
    }

    private void InitializeLocations(Grid grid, Locations locations)
    {
        for (int z = 0; z < grid.Height; z++)
        {
            for (int x = 0; x < grid.Height; x++)
            {
                var locEntity = _world.NewEntity();

                locEntity.Replace(Coords.FromOffset(x, z));
                locEntity.Replace(new HasBaseTerrain(Data.Instance.Terrains["Gg"].Copy()));
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