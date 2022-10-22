using System.Linq;
using System.Collections.Generic;
using Godot;
using RelEcs;

public static class ScenarioSystems
{
    public static void LoadScenarios(World world)
    {
        var scenarioData = new ScenarioData();
        world.AddOrReplaceElement(scenarioData);

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

    public static void SpawnSchedule(World world, string name, int index)
    {
        var scene = world.GetTree().CurrentScene;
        var data = world.GetElement<ScenarioData>();
        var schedule = data.Schedules[name].Instantiate<Schedule>();
        scene.AddChild(schedule);
        world.AddElement(schedule);

        schedule.Set(index);
    }

    public static void SpawnPlayer(World world, int playerId, int side, Coords coords, string faction, int gold)
    {
        var scenario = world.GetElement<Scenario>();
        var data = world.GetElement<UnitData>();

        var username = "Username";

        if (world.TryGetElement<MatchPlayers>(out var matchPlayers))
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

        var sideEntity = world.Spawn()
            .Add(new PlayerId { Value = playerId })
            .Add(new Name { Value = username })
            .Add(new Side { Value = side })
            .Add(new Gold { Value = gold })
            .Add(new Faction { Value = factionData.Name })
            .Add(new Recruits(factionData.Recruits))
            .Id();

        scenario.Sides.Add(side, sideEntity);

        // world.Send(new SpawnUnitTrigger(side, factionData.Leaders[0], coords, true));
    }

    public static void EndTurn(World world)
    {
        var scenario = world.GetElement<Scenario>();

        scenario.EndTurn();

        if (scenario.HasRoundChanged())
        {
            var schedule = world.GetElement<Schedule>();
            schedule.Next();
        }

        var sides = world.Query<Gold, Side, PlayerId>();

        var sideEntity = scenario.GetCurrentSideEntity();

        var (gold, side, playerId) = sides.Get(sideEntity);

        var units = world.Query<Entity, Side, Actions, Moves, Level>();
        var suspends = world.QueryBuilder().Has<Suspended>().Build();

        foreach (var (unitEntity, unitSide, actions, moves, level) in units)
        {
            if (side.Value != unitSide.Value) continue;

            actions.Restore();
            moves.Restore();

            if (suspends.Has(unitEntity))
            {
                world.On(unitEntity).Remove<Suspended>();
            }

            gold.Value -= level.Value;
        }

        foreach (var (village, villageSide) in world.Query<Village, IsCapturedBySide>())
        {
            if (side.Value == villageSide.Value)
            {
                gold.Value += village.List.Count;
            }
        }

        if (world.TryGetElement<TurnPanel>(out var turnPanel))
        {
            var localPlayer = world.GetElement<LocalPlayer>();

            if (playerId.Value == localPlayer.Id)
            {
                Sfx.Instance.Play("TurnBell");
                turnPanel.EndTurnButton.Disabled = false;
            }
            else
            {
                turnPanel.EndTurnButton.Disabled = true;
            }
        }

        var tiles = world.Query<Entity, BaseTerrainSlot, OverlayTerrainSlot, UnitSlot>();
        var heals = world.QueryBuilder().Has<Heals>().Build();

        var healthsAndSides = world.Query<Coords, Health, Side>();

        foreach (var (tileEntity, baseTerrain, overlayTerrain, unit) in tiles)
        {
            var canHeal = heals.Has(baseTerrain.Entity);

            if (world.IsAlive(overlayTerrain.Entity))
            {
                canHeal = canHeal || heals.Has(overlayTerrain.Entity);
            }

            var (coords, health, unitSide) = healthsAndSides.Get(unit.Entity);

            if (!canHeal || unitSide.Value != side.Value || health.IsFull()) continue;

            var diff = Mathf.Min(health.GetDifference(), 8);

            health.Increase(diff);

            SpawnFloatingLabel(world, coords.ToWorld() + Godot.Vector3.Up * 7f, diff.ToString(), Colors.Green);
        }
    }
}