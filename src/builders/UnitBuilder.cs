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

    public UnitBuilder WithHealth(int hp)
    {
        _entity.Add(new Attribute<Health>(hp));
        return this;
    }

    public UnitBuilder WithExperience(int xp)
    {
        _entity.Add(new Attribute<Experience>(xp));
        return this;
    }

    public UnitBuilder WithActions(int ap)
    {
        _entity.Add(new Attribute<Actions>(ap));
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