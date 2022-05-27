using Godot;
using RelEcs;
using RelEcs.Godot;

public class LoadMapEvent
{
    public string Name { get; }

    public LoadMapEvent(string name)
    {
        Name = name;
    }
}

public class LoadMapEventSystem : ISystem
{
    const string Path = "res://data/maps/";

    public void Run(Commands commands)
    {
        commands.Receive((LoadMapEvent e) =>
        {
            var mapData = Loader.LoadJson<MapData>(Path + e.Name + ".json");
            commands.Send(new SpawnMapTrigger(mapData));
        });
    }
}
