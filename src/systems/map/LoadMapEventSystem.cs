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
        world.ForEach((ref LoadMapEvent e) =>
        {
            var mapData = Loader.LoadJson<MapData>(Path + e.Name + ".json");
            world.Spawn().Add(new SpawnMapEvent(mapData));
        });
    }
}
