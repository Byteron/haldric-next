using System.Collections.Generic;
using Godot;
using RelEcs;

public static class ScenarioExtensions
{
    public static void LoadScenarios(this ISystem system)
    {
        var scenarioData = new ScenarioData();
        system.AddOrReplaceElement(scenarioData);

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