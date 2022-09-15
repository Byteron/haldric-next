using System.Collections.Generic;
using RelEcs;
using Godot;

public class LoadScenarioDataTrigger
{
}

public class LoadScenarioDataSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        foreach (var _ in this.Receive<LoadScenarioDataTrigger>())
        {
            var scenarioData = new ScenarioData();
            this.AddOrReplaceElement(scenarioData);

            foreach (var data in Loader.LoadDir("res://data/schedules", new List<string>() { "tscn" }))
            {
                scenarioData.Schedules.Add(data.Id, (PackedScene)data.Data);
            }

            foreach (var data in Loader.LoadDir("res://data/maps", new List<string>() { "json" }, false))
            {
                var mapData = Loader.LoadJson<MapData>(data.Path);
                scenarioData.Maps.Add(data.Id, mapData);
            }
        }
    }
}