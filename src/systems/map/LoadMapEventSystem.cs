using Godot;
using Bitron.Ecs;

public struct LoadMapEvent
{
    public string Name { get; set; }

    public LoadMapEvent(string name)
    {
        Name = name;
    }
}

public class LoadMapEventSystem : IEcsSystem
{
    public static string Path = "res://data/maps/";

    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<LoadMapEvent>().End();

        foreach (var eventEntityId in eventQuery)
        {
            var loadEvent = world.Entity(eventEntityId).Get<LoadMapEvent>();

            var mapData = Loader.LoadJson<MapData>(Path + loadEvent.Name + ".json");

            world.Spawn().Add(new SpawnMapEvent(mapData));
        }
    }
}
