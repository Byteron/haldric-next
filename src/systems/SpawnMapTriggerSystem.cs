using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public class Location
{
}

public class SpawnMapTrigger
{
    public MapData MapData { get; }

    public SpawnMapTrigger(MapData mapData) => MapData = mapData;
    public SpawnMapTrigger() : this(40, 40) { }

    public SpawnMapTrigger(int width, int height)
    {
        MapData = new MapData
        {
            Width = width,
            Height = height
        };
    }
}

public class SpawnMapTriggerSystem : ISystem
{
    Commands _commands;
    Node _parent;

    public SpawnMapTriggerSystem(Node parent) { this._parent = parent; }

    public void Run(Commands commands)
    {
        this._commands = commands;

        commands.Receive((SpawnMapTrigger e) =>
        {
            var mapData = e.MapData;

            if (mapData.Locations.Count == 0)
            {
                mapData = GetMapDataFromDimensions(mapData.Width, mapData.Height);
            }

            commands.AddElement(new ShaderData(mapData.Width, mapData.Height));

            var terrainHighlighter = Scenes.Instantiate<TerrainHighlighter>();
            _parent.AddChild(terrainHighlighter);
            commands.AddElement(terrainHighlighter);

            var players = mapData.Players;
            var map = CreateMapFromMapData(mapData);

            commands.AddElement(map);

            var locations = map.Locations;
            var grid = map.Grid;
            var chunkSize = map.ChunkSize;

            InitializeHover();

            InitializeChunks(chunkSize, grid, locations);
            InitializeNeighbors(locations);
            InitializeCastles(locations);
            InitializeVillages(locations);
            InitializePathFinding(map);
            InitializePlayers(locations, players);
            SendUpdateMapEvent();
        });
    }

    static void InitializePlayers(Locations locations, List<MapDataPlayer> players)
    {
        foreach (var player in players)
        {
            locations.Dict[player.Coords.Cube()].Add(new IsStartingPositionOfSide { Value = player.Side });
        }
    }

    void InitializeHover()
    {
        var cursorView = Scenes.Instantiate<Cursor3D>();
        _parent.AddChild(cursorView);

        _commands.AddElement(cursorView);
    }

    static MapData GetMapDataFromDimensions(int width, int height)
    {
        var mapData = new MapData
        {
            Width = width,
            Height = height
        };

        for (var z = 0; z < height; z++)
        {
            for (var x = 0; x < width; x++)
            {
                var coords = Coords.FromOffset(x, z);

                var locData = new MapDataLocation
                {
                    Coords = coords,
                    Terrain = new List<string>() { "Gg" },
                    Elevation = 0
                };

                mapData.Locations.Add(locData);
            }
        }

        return mapData;
    }

    Map CreateMapFromMapData(MapData mapData)
    {
        var width = mapData.Width;
        var height = mapData.Height;

        var map = new Map(width, height, 4);

        var locations = map.Locations;

        foreach (var locData in mapData.Locations)
        {
            var coords = locData.Coords;

            var locEntity = _commands.Spawn();

            locEntity.Add<Location>();
            locEntity.Add(new Index { Value = (int)coords.Offset().z * width + (int)coords.Offset().x });
            locEntity.Add(coords);

            locEntity.Add(new PlateauArea(0.75f));

            var terrainCodes = locData.Terrain;
            var elevation = locData.Elevation;

            var baseTerrainEntity = Data.Instance.Terrains[(string)terrainCodes[0]];

            locEntity.Add(new Elevation { Value = elevation });

            locEntity.Add(new HasBaseTerrain { Entity = baseTerrainEntity });

            if (terrainCodes.Count == 2)
            {
                var overlayTerrainEntity = Data.Instance.Terrains[(string)terrainCodes[1]];
                locEntity.Add(new HasOverlayTerrain { Entity = overlayTerrainEntity });
            }

            locEntity.Add(new Distance());
            locEntity.Add<PathFrom>();

            locations.Set(coords.Cube(), locEntity);
        }

        return map;
    }

    void InitializeChunks(Vector2i chunkSize, Grid grid, Locations locations)
    {
        var chunks = new Dictionary<Vector3i, Entity>();

        for (var z = 0; z < grid.Height; z++)
        {
            for (var x = 0; x < grid.Height; x++)
            {
                var coords = Coords.FromOffset(x, z);

                var chunkCell = (coords.Offset() / new Vector3(chunkSize.x, 0f, chunkSize.y));
                var chunkCelli = new Vector3i((int)chunkCell.x, 0, (int)chunkCell.z);

                if (!chunks.ContainsKey(chunkCelli))
                {
                    var newChunk = _commands.Spawn().Add<Locations>().Add<Cell>();
                    chunks.Add(chunkCelli, newChunk);
                }

                var locEntity = locations.Get(coords.Cube());
                locEntity.Add(new Cell(chunkCelli));

                var chunkEntity = chunks[chunkCelli];
                var chunkCellComponent = chunkEntity.Get<Cell>();

                chunkCellComponent.Value = chunkCelli;

                var chunkLocations = chunkEntity.Get<Locations>();

                chunkLocations.Set(coords.Cube(), locEntity);
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

            chunkEntity.Add(terrainCollider);
            chunkEntity.Add(terrainFeaturePopulator);
            chunkEntity.Add(terrainMesh);
        }
    }

    static void InitializeNeighbors(Locations locations)
    {
        foreach (var locEntity in locations.Values)
        {
            locEntity.Add<Neighbors>();

            var coords = locEntity.Get<Coords>();
            var neighbors = locEntity.Get<Neighbors>();
            for (var i = 0; i < 6; i++)
            {
                var direction = (Direction)i;

                var nCell = Hex.GetNeighbor(coords.Cube(), direction);

                if (!locations.Has(nCell)) continue;

                var nEntity = locations.Get(nCell);
                neighbors.Set(direction, nEntity);
            }
        }
    }

    static void InitializeCastles(Locations locations)
    {
        foreach (var locEntity in locations.Values)
        {
            var baseTerrainEntity = locEntity.Get<HasBaseTerrain>().Entity;

            if (!baseTerrainEntity.Has<CanRecruitFrom>()) continue;

            var castle = new Castle
            {
                List = FindConnectedLocationsWith<CanRecruitTo>(locEntity)
            };

            locEntity.Add(castle);
        }
    }

    static void InitializeVillages(Locations locations)
    {
        foreach (var locEntity in locations.Values)
        {
            if (!locEntity.Has<HasOverlayTerrain>()) continue;

            var overlayTerrainEntity = locEntity.Get<HasOverlayTerrain>().Entity;

            if (!overlayTerrainEntity.Has<IsCapturable>()) continue;

            var village = new Village
            {
                List = FindConnectedLocationsWith<GivesIncome>(locEntity)
            };

            locEntity.Add(village);
        }
    }

    static List<Entity> FindConnectedLocationsWith<CT>(Entity locEntity) where CT : class
    {
        var list = new List<Entity>();
        var frontier = new Queue<Entity>();
        frontier.Enqueue(locEntity);

        while (frontier.Count > 0)
        {
            var cLocEntity = frontier.Dequeue();
            var cNeighbors = cLocEntity.Get<Neighbors>();

            foreach (var nLocEntity in cNeighbors.Array)
            {
                if (!nLocEntity.IsAlive) continue;

                if (list.Contains(nLocEntity)) continue;

                var nBaseTerrainEntity = nLocEntity.Get<HasBaseTerrain>().Entity;

                var hasT = nBaseTerrainEntity.Has<CT>();

                if (nLocEntity.Has<HasOverlayTerrain>())
                {
                    var nOverlayTerrainEntity = nLocEntity.Get<HasOverlayTerrain>().Entity;
                    hasT = hasT || nOverlayTerrainEntity.Has<CT>();
                }

                if (!hasT) continue;

                frontier.Enqueue(nLocEntity);
                list.Add(nLocEntity);
            }
        }

        return list;
    }

    static void InitializePathFinding(Map map)
    {
        foreach (var locEntity in map.Locations.Dict.Values)
        {
            var coords = locEntity.Get<Coords>();

            map.PathFinder.AddPoint(coords.GetIndex(map.Grid.Width), coords.Cube(), 1);
        }

        foreach (var locEntity in map.Locations.Dict.Values)
        {
            var coords = locEntity.Get<Coords>();
            var neighbors = locEntity.Get<Neighbors>();

            foreach (var nLocEntity in neighbors.Array)
            {
                if (nLocEntity is null || !nLocEntity.IsAlive)
                {
                    continue;
                }

                var nCoords = nLocEntity.Get<Coords>();

                map.PathFinder.ConnectPoints(coords.GetIndex(map.Grid.Width), nCoords.GetIndex(map.Grid.Width), false);
            }
        }
    }

    void SendUpdateMapEvent() { _commands.Send(new UpdateMapTrigger()); }
}