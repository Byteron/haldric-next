using Godot;
using Leopotam.Ecs;

struct Health
{
    public int Value;

    public Health(int value)
    {
        Value = value;
    }
}

struct Experience
{
    public int Value;

    public Experience(int value)
    {
        Value = value;
    }
}

struct Moves
{
    public int Value;

    public Moves(int value)
    {
        Value = value;
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
        _entity = Main.Instance.World.NewEntity();
        return this;
    }

    public UnitBuilder WithId(string id)
    {
        _entity.Replace(new Id(id));
        return this;
    }

    public UnitBuilder WithHealth(int hp)
    {
        _entity.Replace(new Health(hp));
        return this;
    }

    public UnitBuilder WithExperience(int xp)
    {
        _entity.Replace(new Experience(xp));
        return this;
    }

    public UnitBuilder WithMoves(int mp)
    {
        _entity.Replace(new Moves(mp));
        return this;
    }

    public UnitBuilder WithView(string path)
    {
        var packedScene = GD.Load<PackedScene>(path);
        _entity.Replace(new AssetHandle<PackedScene>(packedScene));
        return this;
    }

    public EcsEntity Build()
    {
        return _entity;
    }
}