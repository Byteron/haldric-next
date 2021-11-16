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

            ref var saveMapEvent = ref world.Entity(eventEntityId).Get<SaveMapEvent>();

            var saveData = new Dictionary();
            var locationsData = new Dictionary();
            var playersData = new Dictionary();

            var locations = map.Locations;
            var grid = map.Grid;

            foreach (var item in locations.Dict)
            {
                var cell = item.Key;
                var locEntity = item.Value;

                var terrainCodes = new List<string>();

                ref var baseTerrain = ref locEntity.Get<HasBaseTerrain>();
                var entity = baseTerrain.Entity;
                ref var baseTerrainCode = ref entity.Get<TerrainCode>();

                terrainCodes.Add(baseTerrainCode.Value);

                if (locEntity.Has<HasOverlayTerrain>())
                {
                    ref var overlayTerrain = ref locEntity.Get<HasOverlayTerrain>();
                    var overlayEntity = overlayTerrain.Entity;
                    ref var overlayTerrainCode = ref overlayEntity.Get<TerrainCode>();

                    terrainCodes.Add(overlayTerrainCode.Value);
                }

                var locationData = new Dictionary
                {
                    { "Terrain", terrainCodes },
                    { "Elevation", locEntity.Get<Elevation>().Value }
                };
                if (locEntity.Has<IsStartingPositionOfSide>())
                {
                    ref var startPos = ref locEntity.Get<IsStartingPositionOfSide>();
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
        var jsonString = json.Stringify(saveData);
        var file = new File();
        file.Open(Path + name + ".json", File.ModeFlags.Write);
        file.StoreString(jsonString);
        file.Close();
    }
}