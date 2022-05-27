using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;
using Nakama.TinyJson;

public class SaveMapTrigger
{
    public string Name { get; set; }

    public SaveMapTrigger(string name)
    {
        Name = name;
    }
}

public class SaveMapTriggerSystem : ISystem
{
    const string Path = "res://data/maps/";

    public void Run(Commands commands)
    {
        commands.Receive((SaveMapTrigger e) =>
        {
            var map = commands.GetElement<Map>();
            var grid = map.Grid;

            var mapData = new MapData
            {
                Width = grid.Width,
                Height = grid.Height
            };

            var query = commands.Query<Entity, Coords, Elevation, HasBaseTerrain>().Has<Location>();

            foreach (var (entity, coords, elevation, baseTerrain) in query)
            {
                var locData = new MapDataLocation
                {
                    Coords = coords,
                    Elevation = elevation.Value
                };

                var baseTerrainCode = baseTerrain.Entity.Get<TerrainCode>();

                locData.Terrain.Add(baseTerrainCode.Value);

                if (entity.Has<HasOverlayTerrain>())
                {
                    var overlayTerrain = entity.Get<HasOverlayTerrain>();
                    var overlayTerrainCode = overlayTerrain.Entity.Get<TerrainCode>();

                    locData.Terrain.Add(overlayTerrainCode.Value);
                }

                if (entity.Has<IsStartingPositionOfSide>())
                {
                    var playerMapData = new MapDataPlayer();
                    playerMapData.Coords = coords;
                    playerMapData.Side = entity.Get<IsStartingPositionOfSide>().Value;
                    mapData.Players.Add(playerMapData);
                }

                mapData.Locations.Add(locData);
            }

            SaveToFile(e.Name, mapData);
        });
    }

    static void SaveToFile(string name, MapData mapData)
    {
        var file = new File();
        file.Open(Path + name + ".json", File.ModeFlags.Write);
        file.StoreString(mapData.ToJson());
        file.Close();
    }
}