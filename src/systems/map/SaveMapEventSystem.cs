using System.Collections.Generic;
using Godot;
using Bitron.Ecs;
using Nakama.TinyJson;

public struct SaveMapEvent
{
    public string Name { get; set; }

    public SaveMapEvent(string name)
    {
        Name = name;
    }
}

public class SaveMapEventSystem : IEcsSystem
{
    public static string Path = "res://data/maps/";

    public void Run(EcsWorld world)
    {
        world.ForEach((ref SaveMapEvent e) =>
        {
            var map = world.GetResource<Map>();
            var grid = map.Grid;

            var mapData = new MapData();

            mapData.Width = grid.Width;
            mapData.Height = grid.Height;

            world.ForEach((EcsEntity locEntity, ref Coords coords, ref Elevation elevation, ref HasBaseTerrain baseTerrain, ref Location loc) =>
            {
                var locData = new MapDataLocation();
                locData.Coords = coords;
                locData.Elevation = elevation.Value;

                ref var baseTerrainCode = ref baseTerrain.Entity.Get<TerrainCode>();

                locData.Terrain.Add(baseTerrainCode.Value);

                if (locEntity.Has<HasOverlayTerrain>())
                {
                    ref var overlayTerrain = ref locEntity.Get<HasOverlayTerrain>();
                    ref var overlayTerrainCode = ref overlayTerrain.Entity.Get<TerrainCode>();

                    locData.Terrain.Add(overlayTerrainCode.Value);
                }

                if (locEntity.Has<IsStartingPositionOfSide>())
                {
                    var playerMapData = new MapDataPlayer();
                    playerMapData.Coords = coords;
                    playerMapData.Side = locEntity.Get<IsStartingPositionOfSide>().Value;
                    mapData.Players.Add(playerMapData);
                }

                mapData.Locations.Add(locData);
            });

            SaveToFile(e.Name, mapData);
        });
    }

    private void SaveToFile(string name, MapData mapData)
    {
        var file = new File();
        file.Open(Path + name + ".json", File.ModeFlags.Write);
        file.StoreString(mapData.ToJson());
        file.Close();
    }
}