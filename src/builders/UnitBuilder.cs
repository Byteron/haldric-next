using Godot;
using Bitron.Ecs;

struct Health { } // For Attribute<T>

struct Experience { } // For Attribute<T>

struct Moves { } // For Attribute<T>

struct Attribute<T>
{
    public int Value;
    public int Max;

    public Attribute(int max)
    {
        Max = max;
        Value = Max;
    }

    public void Increase(int amount)
    {
        int sum = Value + amount;
        Value = sum > Max ? Max : sum;
    }
}

struct Id
{
    public string Value;

    public Id(string value)
    {
        Value = value;
    }
}

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

    public UnitBuilder WithMoves(int mp)
    {
        _entity.Add(new Attribute<Moves>(mp));
        return this;
    }

    public UnitBuilder WithView(string path)
    {
        var packedScene = GD.Load<PackedScene>(path);
        _entity.Add(new AssetHandle<PackedScene>(packedScene));
        return this;
    }

    public EcsEntity Build()
    {
        return _entity;
    }
}