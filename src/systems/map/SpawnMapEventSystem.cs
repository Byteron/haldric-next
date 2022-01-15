using System.Collections.Generic;
using Godot;
using Bitron.Ecs;

public struct SpawnMapEvent
{
    public MapData MapData { get; set; }

    public SpawnMapEvent(MapData mapData)
    {
        MapData = mapData;
    }

    public SpawnMapEvent(int width, int height)
    {
        MapData = new MapData();
        MapData.Width = width;
        MapData.Height = height;
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
            var spawnEvent = world.Entity(eventEntity).Get<SpawnMapEvent>();
            var mapData = spawnEvent.MapData;

            if (mapData.Locations.Count == 0)
            {
                mapData = GetMapDataFromDimensions(mapData.Width, mapData.Height);
            }

            world.AddResource(new ShaderData(mapData.Width, mapData.Height));

            var terrainHighlighter = Scenes.Instantiate<TerrainHighlighter>();
            _parent.AddChild(terrainHighlighter);
            world.AddResource(terrainHighlighter);

            var players = mapData.Players;
            Map map = CreateMapFromMapData(mapData);

            world.AddResource(map);

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
        }
    }

    private void InitializePlayers(Locations locations, List<MapDataPlayer> players)
    {
        foreach (var player in players)
        {
            locations.Dict[player.Coords.Cube()].Add(new IsStartingPositionOfSide(player.Side));
        }
    }

    private void InitializeHover()
    {
        var cursorView = Scenes.Instantiate<Cursor3D>();
        _parent.AddChild(cursorView);

        _world.AddResource(cursorView);
    }

    private MapData GetMapDataFromDimensions(int width, int height)
    {
        var mapData = new MapData();
        mapData.Width = width;
        mapData.Height = height;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                var coords = Coords.FromOffset(x, z);

                var locData = new MapDataLocation();
                locData.Coords = coords;
                locData.Terrain = new List<string>() { "Gg" };
                locData.Elevation = 0;

                mapData.Locations.Add(locData);
            }
        }

        return mapData;
    }

    private Map CreateMapFromMapData(MapData mapData)
    {
        var width = mapData.Width;
        var height = mapData.Height;

        var map = new Map(width, height, 4);

        var locations = map.Locations;

        foreach (var locData in mapData.Locations)
        {
            var locEntity = _world.Spawn();

            var coords = locData.Coords;

            locEntity.Add(new Index((int)coords.Offset().z * width + (int)coords.Offset().x));
            locEntity.Add(coords);

            locEntity.Add(new PlateauArea(0.75f));

            var terrainCodes = locData.Terrain;
            var elevation = locData.Elevation;
            var elevationOffset = 0;

            var baseTerrainEntity = Data.Instance.Terrains[(string)terrainCodes[0]];

            locEntity.Add(new Elevation(elevation, elevationOffset));

            locEntity.Add(new HasBaseTerrain(baseTerrainEntity));

            if (terrainCodes.Count == 2)
            {
                var overlayTerrainEntity = Data.Instance.Terrains[(string)terrainCodes[1]];
                locEntity.Add(new HasOverlayTerrain(overlayTerrainEntity));
            }

            locEntity.Add<Distance>();
            locEntity.Add<PathFrom>();

            locations.Set(coords.Cube(), locEntity);
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

                var chunkCell = (coords.Offset() / new Vector3(chunkSize.x, 0f, chunkSize.y));
                var chunkCelli = new Vector3i((int)chunkCell.x, 0, (int)chunkCell.z);

                if (!chunks.ContainsKey(chunkCelli))
                {
                    var newChunk = _world.Spawn().Add<Locations>().Add<Vector3i>();
                    chunks.Add(chunkCelli, newChunk);
                }

                var locEntity = locations.Get(coords.Cube());
                locEntity.Add(chunkCelli);

                var chunkEntity = chunks[chunkCelli];
                ref var chunkCellComponent = ref chunkEntity.Get<Vector3i>();

                chunkCellComponent = chunkCelli;


                ref var chunkLocations = ref chunkEntity.Get<Locations>();

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
            for (int i = 0; i < 6; i++)
            {
                var direction = (Direction)i;

                Vector3 nCell = Hex.GetNeighbor(coords.Cube(), direction);

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

            var castle = new Castle();
            castle.List = FindConnectedLocationsWith<CanRecruitTo>(locEntity);
            locEntity.Add(castle);
        }
    }

    private void InitializeVillages(Locations locations)
    {
        foreach (var locEntity in locations.Values)
        {
            if (!locEntity.Has<HasOverlayTerrain>())
            {
                continue;
            }

            var overlayTerrainEntity = locEntity.Get<HasOverlayTerrain>().Entity;

            if (!overlayTerrainEntity.Has<IsCapturable>())
            {
                continue;
            }

            var village = new Village();
            village.List = FindConnectedLocationsWith<GivesIncome>(locEntity);
            locEntity.Add(village);
        }
    }

    private List<EcsEntity> FindConnectedLocationsWith<T>(EcsEntity locEntity) where T : struct
    {
        List<EcsEntity> list = new List<EcsEntity>();

        Queue<EcsEntity> frontier = new Queue<EcsEntity>();
        frontier.Enqueue(locEntity);

        while (frontier.Count > 0)
        {
            var cLocEntity = frontier.Dequeue();
            ref var cNeighbors = ref cLocEntity.Get<Neighbors>();

            foreach (var nLocEntity in cNeighbors.Array)
            {
                if (!nLocEntity.IsAlive())
                {
                    continue;
                }

                if (list.Contains(nLocEntity))
                {
                    continue;
                }

                var nBaseTerrainEntity = nLocEntity.Get<HasBaseTerrain>().Entity;

                var hasT = nBaseTerrainEntity.Has<T>();

                if (nLocEntity.Has<HasOverlayTerrain>())
                {
                    var nOverlayTerrainEntity = nLocEntity.Get<HasOverlayTerrain>().Entity;
                    hasT = hasT || nOverlayTerrainEntity.Has<T>();
                }

                if (!hasT)
                {
                    continue;
                }


                frontier.Enqueue(nLocEntity);
                list.Add(nLocEntity);
            }
        }

        return list;
    }

    private void InitializePathFinding(Map map)
    {
        foreach (var locEntity in map.Locations.Dict.Values)
        {
            ref var coords = ref locEntity.Get<Coords>();

            map.PathFinder.AddPoint(coords.GetIndex(map.Grid.Width), coords.Cube(), 1);
        }

        foreach (var locEntity in map.Locations.Dict.Values)
        {
            ref var coords = ref locEntity.Get<Coords>();
            ref var neighbors = ref locEntity.Get<Neighbors>();

            foreach (var nLocEntity in neighbors.Array)
            {
                if (!nLocEntity.IsAlive())
                {
                    continue;
                }

                ref var nCoords = ref nLocEntity.Get<Coords>();

                map.PathFinder.ConnectPoints(coords.GetIndex(map.Grid.Width), nCoords.GetIndex(map.Grid.Width), false);
            }
        }
    }

    private void SendUpdateMapEvent()
    {
        _world.Spawn().Add<UpdateMapEvent>();
    }
}