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
        
        foreach (var eventEntityId in eventQuery)
        {
            var map = world.GetResource<Map>();

            var saveMapEvent = eventQuery.Get<SaveMapEvent>(eventEntityId);

            var saveData = new Dictionary();
            var locationsData = new Dictionary();
            var playersData = new Dictionary();

            ref var locations = ref map.Locations;
            ref var grid = ref map.Grid;

            foreach (var item in locations.Dict)
            {
                var cell = item.Key;
                var locEntity = item.Value;

                var terrainCodes = new List<string>();

                ref var baseTerrainEntity = ref locEntity.Get<HasBaseTerrain>().Entity;
                ref var baseTerrainCode = ref baseTerrainEntity.Get<TerrainCode>();

                terrainCodes.Add(baseTerrainCode.Value);

                if (locEntity.Has<HasOverlayTerrain>())
                {
                    ref var overlayTerrainEntity = ref locEntity.Get<HasOverlayTerrain>().Entity;
                    ref var overlayTerrainCode = ref overlayTerrainEntity.Get<TerrainCode>();

                    terrainCodes.Add(overlayTerrainCode.Value);
                }

                var locationData = new Dictionary();
                locationData.Add("Terrain", terrainCodes);
                locationData.Add("Elevation", locEntity.Get<Elevation>().Value);

                if (locEntity.Has<IsStartingPositionOfTeam>())
                {
                    ref var startPos = ref locEntity.Get<IsStartingPositionOfTeam>();
                    playersData.Add(cell, startPos.Value);
                }

                locationsData.Add(cell, locationData);
            }

            saveData.Add("Width", grid.Width);
            saveData.Add("Height", grid.Height);
            saveData.Add("Locations", locationsData);
            saveData.Add("Players", playersData);

            SaveToFile(saveMapEvent.Name, saveData);
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