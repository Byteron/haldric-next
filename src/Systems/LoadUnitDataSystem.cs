using System.Collections.Generic;
using RelEcs;
using Godot;

public class LoadUnitDataTrigger
{
}

public class LoadUnitDataSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        foreach (var _ in this.Receive<LoadUnitDataTrigger>())
        {
            var unitData = new UnitData();
            this.AddOrReplaceElement(unitData);

            foreach (var data in Loader.LoadDir("res://data/units", new List<string>() { "tscn" }))
            {
                unitData.Units.Add(data.Id, (PackedScene)data.Data);
            }

            foreach (var data in Loader.LoadDir("res://data/factions", new List<string>() { "json" }))
            {
                var faction = Loader.LoadJson<FactionData>(data.Path);
                unitData.Factions.Add(faction.Name, faction);
            }
        }
    }
}