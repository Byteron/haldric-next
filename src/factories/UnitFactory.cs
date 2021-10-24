using Bitron.Ecs;
using Haldric.Wdk;

public class UnitFactory
{
    static UnitBuilder _builder = new UnitBuilder();

    public static EcsEntity CreateFromUnitType(EcsWorld world, UnitType unitType, UnitView unitView)
    {
        _builder
            .Create()
            .WithId(unitType.Id)
            .WithHealth(unitType.Health)
            .WithActions(1)
            .WithMoves(unitType.Moves)
            .WithExperience(unitType.Experience)
            .WithView(unitView);

        if (unitType.Weaknesses != null)
        {
            foreach(var weakness in unitType.Weaknesses)
            {
                _builder.WithWeakness(weakness);
            }
        }

        if (unitType.Resistances != null)
        {
            foreach(var resistance in unitType.Resistances)
            {
                _builder.WithResistance(resistance);
            }
        }

        if (unitType.Calamities != null)
        {
            foreach(var calamity in unitType.Calamities)
            {
                _builder.WithCalamity(calamity);
            }
        }

        if (unitType.Immunities != null)
        {
            foreach(var immunity in unitType.Immunities)
            {
                _builder.WithImmunity(immunity);
            }
        }

        foreach (Attack attack in unitType.Attacks.GetChildren())
        {
            EcsEntity attackEntity = world.Spawn()
                .Add(new Id(attack.Name))
                .Add(new Damage(attack.Damage, attack.DamageType))
                .Add(new Strikes(attack.Strikes))
                .Add(new Range(attack.Range));

            _builder.WithAttack(attackEntity);
        }

        return _builder.Build();
    }
}