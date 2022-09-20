using System.Collections.Generic;
using Godot;
using RelEcs;

public static class UnitExtensions
{
    public static void LoadUnits(this ISystem system)
    {
        var unitData = new UnitData();
        system.AddOrReplaceElement(unitData);

        // foreach (var data in Loader.LoadDir("res://data/units", new List<string>() { "tscn" }))
        // {
        //     unitData.Units.Add(data.Id, (PackedScene)data.Data);
        // }

        foreach (var data in Loader.LoadDir("res://data/factions", new List<string> { "json" }, false))
        {
            var faction = Loader.LoadJson<FactionData>(data.Path);
            unitData.Factions.Add(faction.Name, faction);
        }
    }
    
    public static Entity CreateUnitFromUnitType(this ISystem system, UnitType unitType, Entity entity = null)
    {
        if (entity is null)
        {
            entity = system.Spawn().Id();
        }
        else
        {
            system.On(entity).Remove<Id>()
                .Remove<Level>()
                .Remove<Health>()
                .Remove<Actions>()
                .Remove<Moves>()
                .Remove<Experience>()
                .Remove<Aligned>()
                .Remove<Weaknesses>()
                .Remove<Resistances>()
                .Remove<Calamities>()
                .Remove<Immunities>()
                .Remove<Advancements>()
                .Remove<Attacks>()
                .Remove<Mobility>()
                .Remove<UnitView>();
        }

        var attacks = new Attacks();

        foreach (var node in unitType.Attacks.GetChildren())
        {
            var attack = (Attack)node;
            var attackEntity = system.Spawn()
                .Add(new Id { Value = attack.Name })
                .Add(new Damage { Value = attack.Damage, Type = attack.DamageType })
                .Add(new Strikes { Value = attack.Strikes })
                .Add(new Range { Value = attack.Range })
                .Add(attack.Projectile)
                .Id();

            attacks.Add(attackEntity);
        }

        system.On(entity)
            .Add(new Id { Value = unitType.Id })
            .Add(new Level { Value = unitType.Level })
            .Add(new Health { Max = unitType.Health })
            .Add(new Actions { Max = unitType.Actions })
            .Add(new Moves { Max = unitType.Moves })
            .Add(new Experience { Max = unitType.Experience })
            .Add(new Aligned { Value = unitType.Alignment })
            .Add(new Weaknesses { List = unitType.Weaknesses })
            .Add(new Resistances { List = unitType.Resistances })
            .Add(new Calamities { List = unitType.Calamities })
            .Add(new Immunities { List = unitType.Immunities })
            .Add(new Advancements { List = unitType.Advancements })
            .Add(unitType.UnitView)
            .Add(attacks)
            .Id();

        foreach (var node in unitType.Traits.GetChildren())
        {
            var trait = (Trait)node;
            trait.Apply(system.On(entity));
        }

        return entity;
    }
}