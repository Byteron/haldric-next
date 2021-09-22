using Godot;
using Godot.Collections;
using Leopotam.Ecs;

public struct LoadMapEvent
{
    public string Name;

    public LoadMapEvent(string name)
    {
        Name = name;
    }
}

public class LoadMapEventSystem : IEcsRunSystem
{
    public static string Path = "res://data/maps/";
    EcsWorld _world;
    EcsFilter<LoadMapEvent> _events;
    EcsFilter<Locations, Map> _maps;

    public void Run()
    {
        foreach (var i in _events)
        {
            var eventEntity = _events.GetEntity(i);
            var loadMapEvent = eventEntity.Get<LoadMapEvent>();

            var saveData = LoadFromFile(loadMapEvent.Name);

            SendMapChangeEvents(saveData);
        }
    }

    public Dictionary LoadFromFile(string name)
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

    public void SendMapChangeEvents(Dictionary mapData)
    {
        var destroyEntity = _world.NewEntity();
        destroyEntity.Get<DestroyMapEvent>();

        var createEntity = _world.NewEntity();
        createEntity.Replace(new CreateMapEvent(mapData));
    }
}