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

            var mapEntityRef = _world.Spawn()
                .Add<Map>()
                .Add<Grid>()
                .Add<Locations>()
                .Add(new ChunkSize(4, 4));

            ref var locations = ref mapEntityRef.Get<Locations>();
            ref var grid = ref mapEntityRef.Get<Grid>();
            ref var chunkSize = ref mapEntityRef.Get<ChunkSize>();

            InitializeMapCursor();

            if (createEvent.MapData == null)
            {
                createEvent.MapData = GetMapDataFromDimensions(createEvent.Width, createEvent.Height);
            }

            InitializeFromMapData(mapEntityRef, createEvent.MapData);
            InitializeNeighbors(locations);
            InitializeChunks(chunkSize, grid, locations);

            SendUpdateMapEvent();
        }
    }

    private void InitializeMapCursor()
    {
        var cursorEntityRef = _world.Spawn();
        cursorEntityRef.Add<MapCursor>();
        cursorEntityRef.Add<HoveredLocation>();

        var view = Scenes.Instance.LocationHighlight.Instantiate<Node3D>();

        _parent.AddChild(view);

        cursorEntityRef.Add(new NodeHandle<Node3D>(view));
        cursorEntityRef.Add<Highlighter>();
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

    private void InitializeFromMapData(EcsEntityRef mapEntityRef, Dictionary mapData)
    {
        var width = System.Convert.ToInt32(mapData["Width"]);
        var height = System.Convert.ToInt32(mapData["Height"]);

        mapEntityRef.Add(new Grid(width, height));

        ref var locations = ref mapEntityRef.Get<Locations>();

        var locationsData = (Dictionary)mapData["Locations"];

        foreach (var cellString in locationsData.Keys)
        {
            Vector3 cell = (Vector3)GD.Str2Var("Vector3" + cellString);
            var locEntityRef = Main.Instance.World.Spawn();

            locEntityRef.Add(Coords.FromCube(cell));
            locEntityRef.Add(new PlateauArea(0.75f));

            var locationData = (Dictionary)locationsData[cellString];

            var terrainCodes = (Godot.Collections.Array)locationData["Terrain"];
            var elevation = System.Convert.ToInt32(locationData["Elevation"]);

            locEntityRef.Add(new Elevation(elevation));

            locEntityRef.Add(new HasBaseTerrain(Data.Instance.Terrains[(string)terrainCodes[0]]));

            if (terrainCodes.Count == 2)
            {
                locEntityRef.Add(new HasOverlayTerrain(Data.Instance.Terrains[(string)terrainCodes[1]]));
            }

            locations.Set(cell, _world.PackEntity(locEntityRef.Entity()));
        }
    }

    private void InitializeChunks(ChunkSize chunkSize, Grid grid, Locations locations)
    {
        var chunks = new System.Collections.Generic.Dictionary<Vector3i, EcsPackedEntity>();

        for (int z = 0; z < grid.Height; z++)
        {
            for (int x = 0; x < grid.Height; x++)
            {
                var coords = Coords.FromOffset(x, z);

                var chunkCell = (coords.Offset / chunkSize.ToVector3());
                var chunkCelli = new Vector3i((int)chunkCell.x, 0, (int)chunkCell.z);

                if (!chunks.ContainsKey(chunkCelli))
                {
                    chunks.Add(chunkCelli, _world.PackEntity(_world.SpawnEntity()));
                }

                var chunkPackedEntity = chunks[chunkCelli];
                chunkPackedEntity.Unpack(_world, out var chunkEntity);
                var chunkEntityRef = _world.Entity(chunkEntity);
                ref var chunkLocations = ref chunkEntityRef.Get<Locations>();

                var locPackedEntity = locations.Get(coords.Cube);
                chunkLocations.Set(coords.Cube, locPackedEntity);

                locPackedEntity.Unpack(_world, out var locEntity);
                var locEntityRef = _world.Entity(locEntity);
                
                chunkEntityRef.Add(chunkCelli);
                locEntityRef.Add(chunkCelli);
            }
        }

        foreach (var chunkPackedEntity in chunks.Values)
        {
            var terrainMesh = new TerrainMesh();
            var terrainCollider = new TerrainCollider();
            var terrainFeaturePopulator = new TerrainFeaturePopulator();

            _parent.AddChild(terrainMesh);
            _parent.AddChild(terrainCollider);
            _parent.AddChild(terrainFeaturePopulator);
            
            chunkPackedEntity.Unpack(_world, out var chunkEntity);
            var chunkEntityRef = _world.Entity(chunkEntity);

            chunkEntityRef.Add<Chunk>();
            chunkEntityRef.Add(new NodeHandle<TerrainCollider>(terrainCollider));
            chunkEntityRef.Add(new NodeHandle<TerrainFeaturePopulator>(terrainFeaturePopulator));
            chunkEntityRef.Add(new NodeHandle<TerrainMesh>(terrainMesh));
        }
    }

    private void InitializeNeighbors(Locations locations)
    {
        foreach (var packdEntity in locations.Values)
        {
            packdEntity.Unpack(_world, out var entity);
            var entityRef = _world.Entity(entity);

            ref var coords = ref entityRef.Get<Coords>();
            ref var neighbors = ref entityRef.Get<Neighbors>();

            for (Direction direction = Direction.NE; direction <= Direction.SE; direction++)
            {
                Vector3 nCell = Hex.GetNeighbor(coords.Cube, direction);

                if (!locations.Has(nCell))
                {
                    continue;
                }

                var nPackedEntity = locations.Get(nCell);
                neighbors.Set(direction, nPackedEntity);
            }
        }
    }

    private void SendUpdateMapEvent()
    {
        var updateMapEventEntity = _world.NewEntity();
        updateMapEventEntity.Get<UpdateMapEvent>();
    }
}