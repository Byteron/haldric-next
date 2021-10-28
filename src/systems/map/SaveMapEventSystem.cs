using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Bitron.Ecs;

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
        var eventQuery = world.Query<SaveMapEvent>().End();
        
        foreach (var eventEntityId in eventQuery)
        {
            var map = world.GetResource<Map>();

            var saveMapEvent = eventQuery.Get<SaveMapEvent>(eventEntityId);

            var saveData = new Dictionary();
            var locationsData = new Dictionary();

            var locations = map.Locations;
            var grid = map.Grid;

            foreach (var item in locations.Dict)
            {
                var cell = item.Key;
                var location = item.Value;

                var terrainCodes = new List<string>();

                var baseTerrainEntity = location.Get<HasBaseTerrain>().Entity;
                var baseTerrainCode = baseTerrainEntity.Get<TerrainCode>();

                terrainCodes.Add(baseTerrainCode.Value);

                if (location.Has<HasOverlayTerrain>())
                {
                    var overlayTerrainEntity = location.Get<HasOverlayTerrain>().Entity;
                    var overlayTerrainCode = overlayTerrainEntity.Get<TerrainCode>();

                    terrainCodes.Add(overlayTerrainCode.Value);
                }

                var locationData = new Dictionary();
                locationData.Add("Terrain", terrainCodes);
                locationData.Add("Elevation", location.Get<Elevation>().Value);

                locationsData.Add(cell, locationData);
            }

            saveData.Add("Width", grid.Width);
            saveData.Add("Height", grid.Height);
            saveData.Add("Locations", locationsData);

            SaveToFile(saveMapEvent.Name, saveData);
        }
    }

    private void SaveToFile(string name, Dictionary saveData)
    {
        var json = new JSON();
        var jsonString = json.Stringify(saveData);
        var file = new File();
        file.Open(Path + name + ".json", File.ModeFlags.Write);
        file.StoreString(jsonString);
        file.Close();
    }
}