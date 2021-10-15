using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Bitron.Ecs;

public struct SpawnMapEvent
{
    public int Width;
    public int Height;
    public Dictionary MapData;

    public SpawnMapEvent(Dictionary mapData)
    {
        MapData = mapData;
        Width = (int)(float)mapData["Width"];
        Height = (int)(float)mapData["Height"];
    }

    public SpawnMapEvent(int width, int height)
    {
        Width = width;
        Height = height;
        MapData = null;
    }
}

public class SpawnMapEventSystem : IEcsSystem
{
    EcsWorld _world;
    Node _parent;

    public SpawnMapEventSystem(Node parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        _world = world;

        var query = world.Query<SpawnMapEvent>().End();

        foreach (var eventEntity in query)
        {
            var spawnEvent = query.Get<SpawnMapEvent>(eventEntity);

            if (spawnEvent.MapData == null)
            {
                spawnEvent.MapData = GetMapDataFromDimensions(spawnEvent.Width, spawnEvent.Height);
            }
            
            world.AddResource(new ShaderData(spawnEvent.Width, spawnEvent.Height));

            Map map = CreateMapFromMapData(spawnEvent.MapData);

            world.AddResource(map);

            ref var locations = ref map.Locations;
            ref var grid = ref map.Grid;
            ref var chunkSize = ref map.ChunkSize;

            InitializeHover();

            InitializeChunks(chunkSize, grid, locations);
            InitializeNeighbors(locations);
            InitializeCastles(locations);
            SendUpdateMapEvent();
        }
    }

    private void InitializeHover()
    {
        var hoverEntity = _world.Spawn();
        hoverEntity.Add<HoveredLocation>();

        var cursorView = Scenes.Instance.Cursor3D.Instantiate<Cursor3D>();

        _parent.AddChild(cursorView);

        hoverEntity.Add(new NodeHandle<Cursor3D>(cursorView));
        hoverEntity.Add<Highlighter>();
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

    private Map CreateMapFromMapData(Dictionary mapData)
    {
        var width = System.Convert.ToInt32(mapData["Width"]);
        var height = System.Convert.ToInt32(mapData["Height"]);

        var map = new Map();
        map.Locations = new Locations();
        map.Grid = new Grid(width, height);
        map.ChunkSize = new Vector2i(4, 4);

        ref var locations = ref map.Locations;

        var locationsData = (Dictionary)mapData["Locations"];

        foreach (var cellString in locationsData.Keys)
        {
            Vector3 cell = (Vector3)GD.Str2Var("Vector3" + cellString);
            var locEntity = Main.Instance.World.Spawn();

            var coords = Coords.FromCube(cell);

            locEntity.Add(new Index((int)coords.Offset.z * width + (int)coords.Offset.x));
            locEntity.Add(coords);

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

        return map;
    }

    private void InitializeChunks(Vector2i chunkSize, Grid grid, Locations locations)
    {
        var chunks = new System.Collections.Generic.Dictionary<Vector3i, EcsEntity>();

        for (int z = 0; z < grid.Height; z++)
        {
            for (int x = 0; x < grid.Height; x++)
            {
                var coords = Coords.FromOffset(x, z);

                var chunkCell = (coords.Offset / new Vector3(chunkSize.x, 0f, chunkSize.y));
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

            chunkEntity.Add(new NodeHandle<TerrainCollider>(terrainCollider));
            chunkEntity.Add(new NodeHandle<TerrainFeaturePopulator>(terrainFeaturePopulator));
            chunkEntity.Add(new NodeHandle<TerrainMesh>(terrainMesh));
        }
    }

    private void InitializeNeighbors(Locations locations)
    {
        foreach (var locEntity in locations.Values)
        {
            locEntity.Add<Neighbors>();

            ref var coords = ref locEntity.Get<Coords>();
            ref var neighbors = ref locEntity.Get<Neighbors>();

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

    private void InitializeCastles(Locations locations)
    {
        foreach (var locEntity in locations.Values)
        {
            var baseTerrainEntity = locEntity.Get<HasBaseTerrain>().Entity;

            if (!baseTerrainEntity.Has<CanRecruitFrom>())
            {
                continue;
            }

            locEntity.Add<Keep>();

            ref var keep = ref locEntity.Get<Keep>();

            ref var neighbors = ref locEntity.Get<Neighbors>();

            foreach (var nLocEntity in neighbors.GetArray())
            {
                if (!nLocEntity.IsAlive())
                {
                    continue;
                }

                var nBaseTerrainEntity = nLocEntity.Get<HasBaseTerrain>().Entity;

                if (!nBaseTerrainEntity.Has<CanRecruitTo>())
                {
                    continue;
                }

                keep.List.Add(nLocEntity);
            }
        }
    }

    private void SendUpdateMapEvent()
    {
        _world.Spawn().Add<UpdateMapEvent>();
    }
}