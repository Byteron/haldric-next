using Godot;
using Godot.Collections;
using Leopotam.Ecs;

public struct CreateMapEvent
{
    public Dictionary MapData;

    public CreateMapEvent(Dictionary mapData)
    {
        MapData = mapData;
    }
}

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

public class CreateMapEventSystem : IEcsRunSystem
{
    EcsWorld _world;
    EcsFilter<CreateMapEvent> _events;
    Node _parent;

    public CreateMapEventSystem(Node parent)
    {
        _parent = parent;
    }

    public void Run()
    {
        foreach (var i in _events)
        {
            var eventEntity = _events.GetEntity(i);
            var createEvent = eventEntity.Get<CreateMapEvent>();

            var mapData = createEvent.MapData;
            var mapEntity = _world.NewEntity();

            mapEntity.Get<Map>();

            mapEntity.Replace(new ChunkSize(5, 5));

            ref var locations = ref mapEntity.Get<Locations>();
            ref var grid = ref mapEntity.Get<Grid>();
            ref var chunkSize = ref mapEntity.Get<ChunkSize>();
            ref var chunks = ref mapEntity.Get<Chunks>();

            InitializeFromMapData(mapEntity, mapData);
            InitializeHoveredCoords();
            InitializeNeighbors(locations);
            InitializeChunks(chunks, chunkSize, grid, locations);

            SendUpdateMapEvent();

            eventEntity.Destroy();
        }
    }

    private void InitializeHoveredCoords()
    {
        var hoveredCoordsEntity = _world.NewEntity();
        hoveredCoordsEntity.Get<HoveredCoords>();
    }

    private void InitializeFromMapData(EcsEntity mapEntity, Dictionary mapData)
    {
        var width = System.Convert.ToInt32(mapData["Width"]);
        var height = System.Convert.ToInt32(mapData["Height"]);

        mapEntity.Replace(new Grid(width, height));

        ref var locations = ref mapEntity.Get<Locations>();

        GD.Print(mapData["Locations"].GetType());

        var locationsData = (Dictionary)mapData["Locations"];

        foreach (var cellString in locationsData.Keys)
        {
            GD.Print(cellString);

            Vector3 cell = (Vector3)GD.Str2Var("Vector3" + cellString);
            var locEntity = Main.Instance.World.NewEntity();

            locEntity.Replace(Coords.FromCube(cell));
            locEntity.Replace(new PlateauArea(0.75f));

            var locationData = (Dictionary)locationsData[cellString];

            var terrainCodes = (Godot.Collections.Array)locationData["Terrain"];
            var elevation = System.Convert.ToInt32(locationData["Elevation"]);

            locEntity.Replace(new Elevation(elevation));

            locEntity.Replace(new HasBaseTerrain(Data.Instance.Terrains[(string)terrainCodes[0]]));

            if (terrainCodes.Count == 2)
            {
                locEntity.Replace(new HasOverlayTerrain(Data.Instance.Terrains[(string)terrainCodes[1]]));
            }

            locations.Set(cell, locEntity);
        }
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