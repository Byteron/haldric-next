using Godot;
using RelEcs;
using RelEcs.Godot;

public struct LoadMapEvent
{
    public string Name { get; set; }

    public LoadMapEvent(string name)
    {
        Name = name;
    }
}

public class LoadMapEventSystem : ISystem
{
    public static string Path = "res://data/maps/";

    public void Run(Commands commands)
    {
        commands.Receive((LoadMapEvent e) =>
        {
            var mapData = Loader.LoadJson<MapData>(Path + e.Name + ".json");
            commands.Send(new SpawnMapEvent(mapData));
        });
    }
}
