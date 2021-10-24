using Godot;
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

    public UnitBuilder WithId(string id)
    {
        _entity.Add(new Id(id));
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

    public UnitBuilder WithMoves(int ap)
    {
        _entity.Add(new Attribute<Moves>(ap));
        return this;
    }

    public UnitBuilder WithWeakness(DamageType damageType)
    {
        if (!_entity.Has<Weaknesses>())
        {
            _entity.Add<Weaknesses>();
        }

        ref var weaknesses = ref _entity.Get<Weaknesses>();
        weaknesses.List.Add(damageType);

        return this;
    }

    public UnitBuilder WithResistance(DamageType damageType)
    {
        if (!_entity.Has<Resistances>())
        {
            _entity.Add<Resistances>();
        }

        ref var resistances = ref _entity.Get<Resistances>();
        resistances.List.Add(damageType);
        
        return this;
    }

    public UnitBuilder WithCalamity(DamageType damageType)
    {
        if (!_entity.Has<Calamities>())
        {
            _entity.Add<Calamities>();
        }

        ref var calamities = ref _entity.Get<Calamities>();
        calamities.List.Add(damageType);
        
        return this;
    }

    public UnitBuilder WithImmunity(DamageType damageType)
    {
        if (!_entity.Has<Immunities>())
        {
            _entity.Add<Immunities>();
        }

        ref var immunities = ref _entity.Get<Immunities>();
        immunities.List.Add(damageType);
        
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