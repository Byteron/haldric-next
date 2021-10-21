using Bitron.Ecs;

public class UnitFactory
{
    static UnitBuilder _builder = new UnitBuilder();

    public static EcsEntity CreateFromUnitType(EcsWorld world, UnitType unitType, UnitView unitView)
    {
        _builder
            .Create()
            .WithId(unitType.Attributes.Id)
            .WithHealth(unitType.Attributes.Health)
            .WithActions(unitType.Attributes.Actions)
            .WithExperience(unitType.Attributes.Experience)
            .WithView(unitView);

        foreach (Attack attack in unitType.Attacks.GetChildren())
        {
            EcsEntity attackEntity = world.Spawn()
                .Add(new Id(attack.Name))
                .Add(new Damage(attack.Damage, attack.DamageType))
                .Add(new Strikes(attack.Strikes))
                .Add(new Range(attack.Range))
                .Add(new Costs(attack.Costs));

            _builder.WithAttack(attackEntity);
        }

        return _builder.Build();
    }
}