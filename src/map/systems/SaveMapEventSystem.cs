using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Bitron.Ecs;

public struct SaveMapEvent
{
    public string Name;

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
        var mapQuery = world.Query<Locations>().Inc<Map>().Inc<Grid>().End();
        
        foreach (var eventEntityId in eventQuery)
        {
            foreach (var mapEntityId in mapQuery)
            {
                var saveMapEvent = eventQuery.Get<SaveMapEvent>(eventEntityId);

                var saveData = new Dictionary();
                var locationsData = new Dictionary();

                ref var locations = ref mapQuery.Get<Locations>(mapEntityId);
                ref var grid = ref mapQuery.Get<Grid>(mapEntityId);

                foreach (var item in locations.Dict)
                {
                    var cell = item.Key;
                    var location = item.Value;

                    var terrainCodes = new List<string>();

                    ref var baseTerrainEntity = ref location.Get<HasBaseTerrain>().Entity;
                    ref var baseTerrainCode = ref baseTerrainEntity.Get<TerrainCode>();

                    terrainCodes.Add(baseTerrainCode.Value);

                    if (location.Has<HasOverlayTerrain>())
                    {
                        ref var overlayTerrainEntity = ref location.Get<HasOverlayTerrain>().Entity;
                        ref var overlayTerrainCode = ref overlayTerrainEntity.Get<TerrainCode>();

                        terrainCodes.Add(overlayTerrainCode.Value);
                    }

                    var locationData = new Dictionary();
                    locationData.Add("Terrain", terrainCodes);
                    locationData.Add("Elevation", location.Get<Elevation>().Level);

                    locationsData.Add(cell, locationData);
                }

                saveData.Add("Width", grid.Width);
                saveData.Add("Height", grid.Height);
                saveData.Add("Locations", locationsData);

                SaveToFile(saveMapEvent.Name, saveData);
            }
        }
    }

    private void SaveToFile(string name, Dictionary saveData)
    {
        var json = new JSON();
        var jsonString = (string)json.Stringify(saveData);
        var file = new File();
        file.Open(Path + name + ".json", File.ModeFlags.Write);
        file.StoreString(jsonString);
        file.Close();
    }
}