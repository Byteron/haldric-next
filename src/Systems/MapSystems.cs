using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using RelEcs;

public class TileOf
{
}

public static class MapSystems
{
    public static void SaveMap(World world, string mapName)
    {
        
    }
    
    public static void SpawnMap(World world, int width, int height)
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

                var tileData = new MapDataTile
                {
                    Coords = coords,
                    Terrain = { "Gg" },
                    Elevation = 0
                };

                mapData.Tiles.Add(tileData);
            }
        }

        SpawnMap(world, mapData);
    }

    public static void SpawnMap(World world, MapData mapData)
    {
        if (mapData.Tiles.Count == 0) throw new Exception("Cannot spawn map, no tiles present in MapData");

        var parent = world.GetTree().CurrentScene;

        world.AddElement(new HoveredTile());
        // Create Shader Data

        world.AddElement(new ShaderData(mapData.Width, mapData.Height));

        // Create Terrain Highlighter 

        var terrainHighlighter = Scenes.Instantiate<TerrainHighlighter>();
        parent.AddChild(terrainHighlighter);
        world.AddElement(terrainHighlighter);


        // Create Map Resource

        var terrainData = world.GetElement<TerrainData>();
        var terrainGraphicData = world.GetElement<TerrainGraphicData>();
        var map = new Map(mapData.Width, mapData.Height, 4);

        foreach (var tileData in mapData.Tiles)
        {
            var coords = tileData.Coords;
            var terrainCodes = tileData.Terrain;
            var elevation = tileData.Elevation;

            var baseTerrainEntity = terrainData.TerrainEntities[terrainCodes[0]];
            var baseTerrainSlot = new BaseTerrainSlot { Entity = baseTerrainEntity };
            var overlayTerrainSlot = new OverlayTerrainSlot { Entity = Entity.None };

            if (terrainCodes.Count == 2)
            {
                overlayTerrainSlot.Entity = terrainData.TerrainEntities[terrainCodes[1]];
            }

            var tileEntity = world.Spawn()
                .Add(new Index { Value = coords.GetIndex(mapData.Width) })
                .Add(new Elevation { Value = elevation })
                .Add(new PlateauArea(0.75f))
                .Add(new Neighbors())
                .Add(new Distance())
                .Add(new PathFrom())
                .Add(baseTerrainSlot)
                .Add(overlayTerrainSlot)
                .Add(coords)
                .Id();

            map.Tiles.Set(coords.ToCube(), tileEntity);
        }

        world.AddElement(map);

        // Create Cursor

        var cursorView = Scenes.Instantiate<Cursor3D>();
        parent.AddChild(cursorView);
        world.AddElement(cursorView);

        // Create Chunks

        var tiles = map.Tiles;
        var grid = map.Grid;
        var chunkSize = map.ChunkSize;

        var chunks = new System.Collections.Generic.Dictionary<Vector2I, Entity>();

        for (var z = 0; z < grid.Height; z++)
        {
            for (var x = 0; x < grid.Height; x++)
            {
                var coords = Coords.FromOffset(x, z);
                var chunkCell = coords.GetChunk(chunkSize);

                if (!chunks.ContainsKey(chunkCell))
                {
                    var newChunk = world.Spawn()
                        .Add(new Chunk { Cell = chunkCell })
                        .Id();

                    chunks.Add(chunkCell, newChunk);
                }

                var tileEntity = tiles.Get(coords.ToCube());

                var chunkEntity = chunks[chunkCell];

                world.On(tileEntity).Add<TileOf>(chunkEntity);
            }
        }

        foreach (var chunkEntity in chunks.Values)
        {
            var terrainMesh = new TerrainMesh();
            var terrainCollider = new TerrainCollider();
            var terrainFeaturePopulator = new TerrainProps();

            parent.AddChild(terrainMesh);
            parent.AddChild(terrainCollider);
            parent.AddChild(terrainFeaturePopulator);

            var material = GD.Load<Material>("res://assets/graphics/materials/terrain.tres");
            material.Set("shader_param/textures", terrainGraphicData.TextureArray);
            material.Set("shader_param/normal_textures", terrainGraphicData.NormalTextureArray);
            material.Set("shader_param/roughness_textures", terrainGraphicData.RoughnessTextureArray);
            terrainMesh.MaterialOverride = material;

            world.On(chunkEntity)
                .Add(terrainCollider)
                .Add(terrainFeaturePopulator)
                .Add(terrainMesh);
        }

        // Initialize Neighbors

        foreach (var (coords, neighbors) in world.Query<Coords, Neighbors>())
        {
            for (var i = 0; i < 6; i++)
            {
                var direction = (Direction)i;
                var nCell = Hex.GetNeighbor(coords.ToCube(), direction);

                if (!tiles.Has(nCell)) continue;

                var nEntity = tiles.Get(nCell);
                neighbors.Set(direction, nEntity);
            }
        }

        // Initialize Castles

        var canRecruitFromQuery = world.QueryBuilder().Has<CanRecruitFrom>().Build();

        foreach (var (entity, slot) in world.Query<Entity, BaseTerrainSlot>())
        {
            if (!canRecruitFromQuery.Has(slot.Entity)) continue;

            world.On(entity).Add(new Castle { List = FindConnectedTilesWith<CanRecruitTo>(world, entity) });
        }

        var capturables = world.QueryBuilder().Has<IsCapturable>().Build();

        foreach (var (entity, slot) in world.Query<Entity, OverlayTerrainSlot>())
        {
            if (!capturables.Has(slot.Entity)) continue;

            world.On(entity).Add(new Village
            {
                List = FindConnectedTilesWith<GivesIncome>(world, entity)
            });
        }

        // Initialize Pathfinding

        var coordsQuery = world.Query<Coords>();

        foreach (var coords in coordsQuery)
        {
            map.PathFinder.AddPoint(coords.GetIndex(map.Grid.Width), coords.ToCube(), 1);
        }

        foreach (var (coords, neighbors) in world.Query<Coords, Neighbors>())
        {
            foreach (var nTileEntity in neighbors.Array)
            {
                if (nTileEntity is null || !world.IsAlive(nTileEntity)) continue;

                var nCoords = coordsQuery.Get(nTileEntity);

                map.PathFinder.ConnectPoints(coords.GetIndex(map.Grid.Width), nCoords.GetIndex(map.Grid.Width), false);
            }
        }

        // Initialize Player Positions

        var players = mapData.Players;

        foreach (var player in players)
        {
            var tileEntity = tiles.Dict[player.Coords.ToCube()];
            world.On(tileEntity).Add(new IsStartingPositionOfSide { Value = player.Side });
        }
    }

    public static void DespawnMap(World world)
    {
        world.DespawnAllWith<Coords>();

        foreach (var (entity, mesh, props, collider) in world.Query<Entity, TerrainMesh, TerrainProps, TerrainCollider>())
        {
            mesh.QueueFree();
            props.QueueFree();
            collider.QueueFree();
            world.On(entity).Remove<TerrainCollider>().Remove<TerrainMesh>().Remove<TerrainProps>();
        }

        GD.Print("Terrain Nodes Freed");

        world.DespawnAllWith<Chunk>();

        world.RemoveElement<Map>();
        world.RemoveElement<ShaderData>();
        world.RemoveElement<HoveredTile>();
        world.GetElement<Cursor3D>().QueueFree();
        world.RemoveElement<Cursor3D>();
        world.GetElement<TerrainHighlighter>().QueueFree();
        world.RemoveElement<TerrainHighlighter>();
    }

    public static void UpdateMapCursor(World world)
    {
        if (!world.TryGetElement<HoveredTile>(out var hoveredTile)) return;
        if (!world.TryGetElement<Cursor3D>(out var cursor)) return;
        if (!world.IsAlive(hoveredTile.Entity)) return;
        if (!hoveredTile.HasChanged) return;

        hoveredTile.HasChanged = false;

        var tiles = world.Query<Coords, Elevation>();
        var (coords, elevation) = tiles.Get(hoveredTile.Entity);

        var position = coords.ToWorld();
        position.Y = elevation.Height;

        cursor.Position = position;
    }
    
    static Vector3 previousCell = Vector3.Zero;

    public static void UpdateHoveredTile(World world)
    {
        if (!world.HasElement<Map>()) return;
        if (!world.TryGetElement<HoveredTile>(out var hoveredTile)) return;
        
        var result = ShootRay(world);

        if (!result.ContainsKey("position")) return;

        var position = (Vector3)result["position"];
        var coords = Coords.FromWorld(position);

        if (previousCell == coords.ToAxial()) return;

        var map = world.GetElement<Map>();

        var tileEntity = map.Tiles.Get(coords.ToCube());

        hoveredTile.Entity = tileEntity;
        hoveredTile.Coords = coords;
        hoveredTile.HasChanged = true;
        
        GD.Print(coords);

        previousCell = coords.ToAxial();
    }
    
    static List<Entity> FindConnectedTilesWith<T>(World world, Entity locEntity) where T : class
    {
        var list = new List<Entity>();
        var frontier = new Queue<Entity>();
        frontier.Enqueue(locEntity);

        var neighbors = world.Query<Neighbors>();
        var terrainSlots = world.Query<BaseTerrainSlot, OverlayTerrainSlot>();
        var ts = world.QueryBuilder().Has<T>().Build();

        while (frontier.Count > 0)
        {
            var cTileEntity = frontier.Dequeue();
            var cNeighbors = neighbors.Get(cTileEntity);

            foreach (var nTileEntity in cNeighbors.Array)
            {
                if (!world.IsAlive(nTileEntity)) continue;
                if (list.Contains(nTileEntity)) continue;

                var (nBaseTerrainSlot, nOverlayTerrainSlot) = terrainSlots.Get(nTileEntity);
                var hasT = ts.Has(nBaseTerrainSlot.Entity) || ts.Has(nOverlayTerrainSlot.Entity);

                // if (system.IsAlive(nOverlayTerrainSlot.Entity))
                // {
                //     hasT = hasT || ts.Has(nOverlayTerrainSlot.Entity);
                // }

                if (!hasT) continue;

                frontier.Enqueue(nTileEntity);
                list.Add(nTileEntity);
            }
        }

        return list;
    }
    
    static Dictionary ShootRay(World world)
    {
        var scene = (Node3D)world.GetTree().CurrentScene;
        var spaceState = scene.GetWorld3D().DirectSpaceState;
        var viewport = scene.GetViewport();

        var camera = viewport.GetCamera3D();
        var mousePosition = viewport.GetMousePosition();

        if (camera == null) return new Dictionary();

        var from = camera.ProjectRayOrigin(mousePosition);
        var to = from + camera.ProjectRayNormal(mousePosition) * 1000f;

        var parameters3D = new PhysicsRayQueryParameters3D();
        parameters3D.From = from;
        parameters3D.To = to;

        return spaceState.IntersectRay(parameters3D);
    }
}