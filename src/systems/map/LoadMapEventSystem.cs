using Godot;
using Godot.Collections;
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
			var loadMapEvent = world.Entity(eventEntityId).Get<LoadMapEvent>();

			var saveData = LoadFromFile(loadMapEvent.Name);

			world.Spawn().Add(new SpawnMapEvent(saveData));
		}
	}

	private Dictionary LoadFromFile(string name)
	{
		var file = new File();
		GD.Print(Path + name + ".json");
		
		if (!(file.Open(Path + name + ".json", File.ModeFlags.Read) == Error.Ok))
		{
			GD.PushError("error reading file");
			return new Dictionary();
		}

		var jsonString = file.GetAsText();

		var json = new JSON();
		json.Parse(jsonString);
		return json.GetData() as Dictionary;
	}
}
