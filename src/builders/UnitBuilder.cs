using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public class UnitBuilder
{
    Entity _entity;

    Commands commands;

    public UnitBuilder(Commands commands)
    {
        this.commands = commands;
    }

    public UnitBuilder Create()
    {
        _entity = commands.Spawn();
        _entity.Add<Mobility>();
        return this;
    }

    public UnitBuilder Use(Entity entity)
    {
        _entity = entity;
        return this;
    }

    public UnitBuilder WithId(string id)
    {
        _entity.Add(new Id { Value = id });
        return this;
    }

    public UnitBuilder WithLevel(int level)
    {
        _entity.Add(new Level { Value = level });
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

    public UnitBuilder WithAligned(Alignment value)
    {
        _entity.Add(new Aligned { Value = value });
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

    public UnitBuilder WithAttack(Entity attackEntity)
    {
        if (!_entity.Has<Attacks>())
        {
            _entity.Add<Attacks>();
        }

        var attacks = _entity.Get<Attacks>();
        attacks.Add(attackEntity);
        return this;
    }

    public UnitBuilder WithView(UnitView unitView)
    {
        _entity.Add(unitView);
        return this;
    }

    public Entity Build()
    {
        return _entity;
    }
}