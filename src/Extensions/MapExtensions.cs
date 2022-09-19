using System;
using System.Collections.Generic;
using Godot;
using RelEcs;

public class TileOf
{
}

public static class MapExtensions
{
    public static void SpawnMap(this ISystem system, int width, int height)
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

        system.SpawnMap(mapData);
    }

    public static void SpawnMap(this ISystem system, MapData mapData)
    {
        if (mapData.Tiles.Count == 0) throw new Exception("Cannot spawn map, no tiles present in MapData");

        var parent = system.GetTree().CurrentScene;

        system.AddElement(new HoveredTile());
        // Create Shader Data

        system.AddElement(new ShaderData(mapData.Width, mapData.Height));

        // Create Terrain Highlighter 

        var terrainHighlighter = Scenes.Instantiate<TerrainHighlighter>();
        parent.AddChild(terrainHighlighter);
        system.AddElement(terrainHighlighter);


        // Create Map Resource

        var terrainData = system.GetElement<TerrainData>();
        var terrainGraphicData = system.GetElement<TerrainGraphicData>();
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

            var tileEntity = system.Spawn()
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

        system.AddElement(map);

        // Create Cursor

        var cursorView = Scenes.Instantiate<Cursor3D>();
        parent.AddChild(cursorView);
        system.AddElement(cursorView);

        // Create Chunks

        var tiles = map.Tiles;
        var grid = map.Grid;
        var chunkSize = map.ChunkSize;

        var chunks = new Dictionary<Vector2i, Entity>();

        for (var z = 0; z < grid.Height; z++)
        {
            for (var x = 0; x < grid.Height; x++)
            {
                var coords = Coords.FromOffset(x, z);
                var chunkCell = coords.GetChunk(chunkSize);

                if (!chunks.ContainsKey(chunkCell))
                {
                    var newChunk = system.Spawn()
                        .Add(new Chunk { Cell = chunkCell })
                        .Id();

                    chunks.Add(chunkCell, newChunk);
                }

                var tileEntity = tiles.Get(coords.ToCube());

                var chunkEntity = chunks[chunkCell];

                system.On(tileEntity).Add<TileOf>(chunkEntity);
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

            system.On(chunkEntity)
                .Add(terrainCollider)
                .Add(terrainFeaturePopulator)
                .Add(terrainMesh);
        }

        // Initialize Neighbors

        foreach (var (coords, neighbors) in system.Query<Coords, Neighbors>())
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

        var canRecruitFromQuery = system.QueryBuilder().Has<CanRecruitFrom>().Build();

        foreach (var (entity, slot) in system.Query<Entity, BaseTerrainSlot>())
        {
            if (!canRecruitFromQuery.Has(slot.Entity)) continue;

            system.On(entity).Add(new Castle { List = system.FindConnectedTilesWith<CanRecruitTo>(entity) });
        }

        var capturables = system.QueryBuilder().Has<IsCapturable>().Build();

        foreach (var (entity, slot) in system.Query<Entity, OverlayTerrainSlot>())
        {
            if (!capturables.Has(slot.Entity)) continue;

            system.On(entity).Add(new Village
            {
                List = system.FindConnectedTilesWith<GivesIncome>(entity)
            });
        }

        // Initialize Pathfinding

        var coordsQuery = system.Query<Coords>();

        foreach (var coords in coordsQuery)
        {
            map.PathFinder.AddPoint(coords.GetIndex(map.Grid.Width), coords.ToCube(), 1);
        }

        foreach (var (coords, neighbors) in system.Query<Coords, Neighbors>())
        {
            foreach (var nTileEntity in neighbors.Array)
            {
                if (nTileEntity is null || !system.IsAlive(nTileEntity)) continue;

                var nCoords = coordsQuery.Get(nTileEntity);

                map.PathFinder.ConnectPoints(coords.GetIndex(map.Grid.Width), nCoords.GetIndex(map.Grid.Width), false);
            }
        }

        // Initialize Player Positions

        var players = mapData.Players;

        foreach (var player in players)
        {
            var tileEntity = tiles.Dict[player.Coords.ToCube()];
            system.On(tileEntity).Add(new IsStartingPositionOfSide { Value = player.Side });
        }
    }

    public static void DespawnMap(this ISystem system)
    {
        system.DespawnAllWith<Coords>();

        foreach (var (entity, mesh, props, collider) in system.Query<Entity, TerrainMesh, TerrainProps, TerrainCollider>())
        {
            mesh.QueueFree();
            props.QueueFree();
            collider.QueueFree();
            system.On(entity).Remove<TerrainCollider>().Remove<TerrainMesh>().Remove<TerrainProps>();
        }

        GD.Print("Terrain Nodes Freed");

        system.DespawnAllWith<Chunk>();

        system.RemoveElement<Map>();
        system.RemoveElement<ShaderData>();
        system.RemoveElement<HoveredTile>();
        system.GetElement<Cursor3D>().QueueFree();
        system.RemoveElement<Cursor3D>();
        system.GetElement<TerrainHighlighter>().QueueFree();
        system.RemoveElement<TerrainHighlighter>();
    }

    static List<Entity> FindConnectedTilesWith<T>(this ISystem system, Entity locEntity) where T : class
    {
        var list = new List<Entity>();
        var frontier = new Queue<Entity>();
        frontier.Enqueue(locEntity);

        var neighbors = system.Query<Neighbors>();
        var terrainSlots = system.Query<BaseTerrainSlot, OverlayTerrainSlot>();
        var ts = system.QueryBuilder().Has<T>().Build();

        while (frontier.Count > 0)
        {
            var cTileEntity = frontier.Dequeue();
            var cNeighbors = neighbors.Get(cTileEntity);

            foreach (var nTileEntity in cNeighbors.Array)
            {
                if (!system.IsAlive(nTileEntity)) continue;
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
}