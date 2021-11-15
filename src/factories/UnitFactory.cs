using Bitron.Ecs;
using Haldric.Wdk;
using Godot;

public class UnitFactory
{
    static UnitBuilder _builder = new UnitBuilder();

    public static EcsEntity CreateFromUnitType(EcsWorld world, UnitType unitType, UnitView unitView, EcsEntity entity = default)
    {
        if (entity.IsAlive())
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
                .Remove<NodeHandle<UnitView>>();

            _builder.Use(entity);
        }
        else
        {
            _builder.Create();
        }

        _builder
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
            EcsEntity attackEntity = world.Spawn()
                .Add(new Id(attack.Name))
                .Add(new Damage(attack.Damage, attack.DamageType))
                .Add(new Strikes(attack.Strikes))
                .Add(new Range(attack.Range))
                .Add(new AssetHandle<PackedScene>(attack.Projectile));

            _builder.WithAttack(attackEntity);
        }

        return _builder.Build();
    }
}