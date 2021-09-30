using Godot;
using Godot.Collections;
using Bitron.Ecs;

public struct LoadMapEvent
{
    public string Name;

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
            var loadMapEvent = eventQuery.Get<LoadMapEvent>(eventEntityId);

            var saveData = LoadFromFile(loadMapEvent.Name);

            SendMapChangeEvents(world, saveData);
        }
    }

    private Dictionary LoadFromFile(string name)
    {
        var file = new File();
        GD.Print(Path + name + ".json");
        if (!(file.Open(Path + name + ".json", File.ModeFlags.Read) == Error.Ok))
        {
            GD.Print("error reading file");
            return new Dictionary();
        }

        var jsonString = file.GetAsText();

        var json = new JSON();
        json.Parse(jsonString);
        return json.GetData() as Dictionary;
    }

    private void SendMapChangeEvents(EcsWorld world, Dictionary mapData)
    {
        world.Spawn().Add<DespawnMapEvent>();
        world.Spawn().Add(new SpawnMapEvent(mapData));
    }
}