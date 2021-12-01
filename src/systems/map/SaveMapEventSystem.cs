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
        var eventQuery = world.Query<SaveMapEvent>().End();

        foreach (var eventEntityId in eventQuery)
        {
            ref var saveMapEvent = ref world.Entity(eventEntityId).Get<SaveMapEvent>();

            var map = world.GetResource<Map>();
            var locations = map.Locations;
            var grid = map.Grid;

            var mapData = new MapData();

            mapData.Width = grid.Width;
            mapData.Height = grid.Height;

            foreach (var locEntity in locations.Dict.Values)
            {
                var coords = locEntity.Get<Coords>();

                var locData = MapDataLocation.FromLocEntity(locEntity);

                if (locEntity.Has<IsStartingPositionOfSide>())
                {
                    var playerMapData = new MapDataPlayer();
                    playerMapData.Coords = coords;
                    playerMapData.Side = locEntity.Get<IsStartingPositionOfSide>().Value;
                    mapData.Players.Add(playerMapData);
                }

                mapData.Locations.Add(locData);
            }

            SaveToFile(saveMapEvent.Name, mapData);
        }
    }

    private void SaveToFile(string name, MapData mapData)
    {
        var file = new File();
        file.Open(Path + name + ".json", File.ModeFlags.Write);
        file.StoreString(mapData.ToJson());
        file.Close();
    }
}