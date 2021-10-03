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
            .WithExperience(unitType.Attributes.Experience)
            .WithMoves(unitType.Attributes.Moves)
            .WithView(unitView);

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