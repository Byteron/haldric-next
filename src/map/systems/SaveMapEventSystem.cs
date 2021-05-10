using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Leopotam.Ecs;

public struct SaveMapEvent
{
    public string Name;

    public SaveMapEvent(string name)
    {
        Name = name;
    }
}

public class SaveMapEventSystem : IEcsRunSystem
{
    public static string Path = "res://data/maps/";

    EcsFilter<SaveMapEvent> _events;
    EcsFilter<Locations, Map> _maps;

    public void Run()
    {
        foreach (var i in _events)
        {
            var eventEntity = _events.GetEntity(i);
            var saveMapEvent = eventEntity.Get<SaveMapEvent>();

            var saveData = new Dictionary();
            var locationsData = new Dictionary();

            ref var locations = ref _maps.GetEntity(0).Get<Locations>();
            ref var grid = ref _maps.GetEntity(0).Get<Grid>();

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

            eventEntity.Destroy();
        }
    }

    public void SaveToFile(string name, Dictionary saveData)
    {
        var jsonString = (string)JSON.Print(saveData);
        var file = new File();
        file.Open(Path + name + ".json", File.ModeFlags.Write);
        file.StoreString(jsonString);
        file.Close();
    }
}