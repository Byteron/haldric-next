using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Bitron.Ecs;

public struct CreateMapEvent
{
    public int Width;
    public int Height;
    public Dictionary MapData;

    public CreateMapEvent(Dictionary mapData)
    {
        MapData = mapData;
        Width = 0;
        Height = 0;
    }

    public CreateMapEvent(int width, int height)
    {
        Width = width;
        Height = height;
        MapData = null;
    }
}

public struct Chunk { }
public struct Map { }

public struct MapCursor { }

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

public class CreateMapEventSystem : IEcsSystem
{
    EcsWorld _world;
    Node _parent;

    public CreateMapEventSystem(Node parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        _world = world;

        var query = world.Query<CreateMapEvent>().End();

        foreach (var eventEntity in query)
        {

            var createEvent = query.Get<CreateMapEvent>(eventEntity);

            var mapEntity = _world.Spawn()
                .Add<Map>()
                .Add<Grid>()
                .Add<Locations>()
                .Add(new ChunkSize(4, 4));

            ref var locations = ref mapEntity.Get<Locations>();
            ref var grid = ref mapEntity.Get<Grid>();
            ref var chunkSize = ref mapEntity.Get<ChunkSize>();

            InitializeMapCursor();

            if (createEvent.MapData == null)
            {
                createEvent.MapData = GetMapDataFromDimensions(createEvent.Width, createEvent.Height);
            }

            InitializeFromMapData(mapEntity, createEvent.MapData);
            InitializeNeighbors(locations);
            InitializeChunks(chunkSize, grid, locations);

            SendUpdateMapEvent();
        }
    }

    private void InitializeMapCursor()
    {
        var cursorEntity = _world.Spawn();
        cursorEntity.Add<MapCursor>();
        cursorEntity.Add<HoveredLocation>();

        var view = Scenes.Instance.LocationHighlight.Instantiate<Node3D>();

        _parent.AddChild(view);

        cursorEntity.Add(new NodeHandle<Node3D>(view));
        cursorEntity.Add<Highlighter>();
    }

    private Dictionary GetMapDataFromDimensions(int width, int height)
    {
        var dict = new Dictionary();

        dict.Add("Width", width);
        dict.Add("Height", height);

        var locsDict = new Dictionary();

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                var coords = Coords.FromOffset(x, z);

                var locDict = new Dictionary();

                locDict.Add("Terrain", new List<string>() { "Gg" });
                locDict.Add("Elevation", 0);

                if (locsDict.Contains(coords.Cube))
                {
                    locsDict[coords.Cube] = locDict;
                }
                else
                {
                    locsDict.Add(coords.Cube, locDict);
                }
            }
        }

        dict.Add("Locations", locsDict);
        
        var json = new JSON();
        var jsonString = json.Stringify(dict);
        if (json.Parse(jsonString) != Error.Ok)
        {
            GD.PushError("JSON could not be parsed");
        }

        dict = json.GetData() as Dictionary;

        return dict;
    }

    private void InitializeFromMapData(EcsEntity mapEntity, Dictionary mapData)
    {
        var width = System.Convert.ToInt32(mapData["Width"]);
        var height = System.Convert.ToInt32(mapData["Height"]);

        ref var grid = ref mapEntity.Get<Grid>();
        grid = new Grid(width, height);

        ref var locations = ref mapEntity.Get<Locations>();

        var locationsData = (Dictionary)mapData["Locations"];

        foreach (var cellString in locationsData.Keys)
        {
            Vector3 cell = (Vector3)GD.Str2Var("Vector3" + cellString);
            var locEntity = Main.Instance.World.Spawn();

            locEntity.Add(Coords.FromCube(cell));
            locEntity.Add(new PlateauArea(0.75f));

            var locationData = (Dictionary)locationsData[cellString];

            var terrainCodes = (Godot.Collections.Array)locationData["Terrain"];
            var elevation = System.Convert.ToInt32(locationData["Elevation"]);

            locEntity.Add(new Elevation(elevation));

            locEntity.Add(new HasBaseTerrain(Data.Instance.Terrains[(string)terrainCodes[0]]));

            if (terrainCodes.Count == 2)
            {
                locEntity.Add(new HasOverlayTerrain(Data.Instance.Terrains[(string)terrainCodes[1]]));
            }

            locations.Set(cell, locEntity);
        }
    }

    private void InitializeChunks(ChunkSize chunkSize, Grid grid, Locations locations)
    {
        var chunks = new System.Collections.Generic.Dictionary<Vector3i, EcsEntity>();

        for (int z = 0; z < grid.Height; z++)
        {
            for (int x = 0; x < grid.Height; x++)
            {
                var coords = Coords.FromOffset(x, z);

                var chunkCell = (coords.Offset / chunkSize.ToVector3());
                var chunkCelli = new Vector3i((int)chunkCell.x, 0, (int)chunkCell.z);

                if (!chunks.ContainsKey(chunkCelli))
                {
                    var newChunk = _world.Spawn().Add<Locations>().Add<Vector3i>();
                    chunks.Add(chunkCelli, newChunk);
                }

                var locEntity = locations.Get(coords.Cube);
                locEntity.Add(chunkCelli);
                
                var chunkEntity = chunks[chunkCelli];
                ref var chunkCellComponent = ref chunkEntity.Get<Vector3i>();
                
                chunkCellComponent = chunkCelli;
                

                ref var chunkLocations = ref chunkEntity.Get<Locations>();

                chunkLocations.Set(coords.Cube, locEntity);

                
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

            chunkEntity.Add<Chunk>();
            chunkEntity.Add(new NodeHandle<TerrainCollider>(terrainCollider));
            chunkEntity.Add(new NodeHandle<TerrainFeaturePopulator>(terrainFeaturePopulator));
            chunkEntity.Add(new NodeHandle<TerrainMesh>(terrainMesh));
        }
    }

    private void InitializeNeighbors(Locations locations)
    {
        foreach (var entity in locations.Values)
        {
            entity.Add<Neighbors>();

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
        _world.Spawn().Add<UpdateMapEvent>();
    }
}