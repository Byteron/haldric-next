using System.Linq;
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

    public static void SpawnSchedule(this ISystem system, string name, int index)
    {
        var scene = system.GetTree().CurrentScene;
        var data = system.GetElement<ScenarioData>();
        var schedule = data.Schedules[name].Instantiate<Schedule>();
        scene.AddChild(schedule);
        system.AddElement(schedule);

        schedule.Set(index);
    }

    public static void SpawnPlayer(this ISystem system, int playerId, int side, Coords coords, string faction, int gold)
    {
        var scenario = system.GetElement<Scenario>();
        var data = system.GetElement<UnitData>();

        var username = "Username";

        if (system.TryGetElement<MatchPlayers>(out var matchPlayers))
        {
            username = matchPlayers.Array[playerId].Username;
        }

        FactionData factionData;

        if (faction == "Random")
        {
            var factionName = data.Factions.Keys.ToArray()[GD.Randi() % data.Factions.Count];
            factionData = data.Factions[factionName];
        }
        else
        {
            factionData = data.Factions[faction];
        }

        GD.Print($"Spawning Player -  Id: {playerId} | Name: {username} | Side: {side}");

        var sideEntity = system.Spawn()
            .Add(new PlayerId { Value = playerId })
            .Add(new Name { Value = username })
            .Add(new Side { Value = side })
            .Add(new Gold { Value = gold })
            .Add(new Faction { Value = factionData.Name })
            .Add(new Recruits(factionData.Recruits))
            .Id();

        scenario.Sides.Add(side, sideEntity);

        // system.Send(new SpawnUnitTrigger(side, factionData.Leaders[0], coords, true));
    }
}