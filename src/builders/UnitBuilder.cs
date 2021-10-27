using System.Collections.Generic;
using Bitron.Ecs;
using Haldric.Wdk;

public class UnitBuilder
{
    EcsEntity _entity;

    public UnitBuilder Create()
    {
        _entity = Main.Instance.World.Spawn();
        return this;
    }

    public UnitBuilder Use(EcsEntity entity)
    {
        _entity = entity;
        return this;
    }

    public UnitBuilder WithId(string id)
    {
        _entity.Add(new Id(id));
        return this;
    }

    public UnitBuilder WithLevel(int level)
    {
        _entity.Add(new Level(level));
        return this;
    }

    public UnitBuilder WithActions(int ap)
    {
        _entity.Add(new Attribute<Actions>(ap));
        return this;
    }

    public UnitBuilder WithHealth(int hp)
    {
        _entity.Add(new Attribute<Health>(hp));
        return this;
    }

    public UnitBuilder WithExperience(int xp)
    {
        var experience = new Attribute<Experience>(xp);
        experience.Empty();
        _entity.Add(experience);
        return this;
    }

    public UnitBuilder WithMoves(int mp)
    {
        _entity.Add(new Attribute<Moves>(mp));
        return this;
    }

    public UnitBuilder WithWeaknesses(List<DamageType> types)
    {
        _entity.Add(new Weaknesses(types));
        return this;
    }

    public UnitBuilder WithResistances(List<DamageType> types)
    {
        _entity.Add(new Resistances(types));
        return this;
    }

    public UnitBuilder WithCalamities(List<DamageType> types)
    {
        _entity.Add(new Calamities(types));
        return this;
    }

    public UnitBuilder WithImmunities(List<DamageType> types)
    {
        _entity.Add(new Immunities(types));
        return this;
    }

    public UnitBuilder WithAdvancements(List<string> types)
    {
        _entity.Add(new Advancements(types));
        return this;
    }

    public UnitBuilder WithAttack(EcsEntity attackEntity)
    {
        if (!_entity.Has<Attacks>())
        {
            _entity.Add<Attacks>();
        }

        ref var attacks = ref _entity.Get<Attacks>();
        attacks.Add(attackEntity);
        return this;
    }

    public UnitBuilder WithView(UnitView unitView)
    {
        _entity.Add(new NodeHandle<UnitView>(unitView));
        return this;
    }

    public EcsEntity Build()
    {
        return _entity;
    }
}