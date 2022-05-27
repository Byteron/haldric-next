using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;
using Godot;

public static class UnitFactory
{
    public static Entity CreateFromUnitType(Commands commands, UnitType unitType, UnitView unitView, Entity entity = default)
    {
        var builder = new UnitBuilder(commands);

        if (entity is not null && entity.IsAlive)
        {
            entity.Remove<Id>()
                .Remove<Level>()
                .Remove<Attribute<Health>>()
                .Remove<Attribute<Actions>>()
                .Remove<Attribute<Moves>>()
                .Remove<Aligned>()
                .Remove<Attribute<Experience>>()
                .Remove<Weaknesses>()
                .Remove<Resistances>()
                .Remove<Calamities>()
                .Remove<Immunities>()
                .Remove<Advancements>()
                .Remove<Attacks>()
                .Remove<Mobility>()
                .Remove<UnitView>();

            builder.Use(entity);
        }
        else
        {
            builder.Create();
        }

        builder
            .WithId(unitType.Id)
            .WithLevel(unitType.Level)
            .WithHealth(unitType.Health)
            .WithActions(unitType.Actions)
            .WithMoves(unitType.Moves)
            .WithAligned(unitType.Alignment)
            .WithExperience(unitType.Experience)
            .WithWeaknesses(unitType.Weaknesses)
            .WithResistances(unitType.Resistances)
            .WithCalamities(unitType.Calamities)
            .WithImmunities(unitType.Immunities)
            .WithAdvancements(unitType.Advancements)
            .WithView(unitView);

        foreach (Attack attack in unitType.Attacks.GetChildren())
        {
            var attackEntity = commands.Spawn()
                .Add(new Id { Value = attack.Name })
                .Add(new Damage(attack.Damage, attack.DamageType))
                .Add(new Strikes { Value = attack.Strikes })
                .Add(new Range { Value = attack.Range })
                .Add(attack.Projectile);

            builder.WithAttack(attackEntity);
        }

        var unitEntity = builder.Build();
        
        foreach (Trait trait in unitType.Traits.GetChildren())
        {
            trait.Apply(unitEntity);
        }

        return unitEntity;
    }
}