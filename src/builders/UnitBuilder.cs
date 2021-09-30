using Godot;
using Bitron.Ecs;

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