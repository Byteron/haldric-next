using Godot;
using System.Collections.Generic;
using Leopotam.Ecs;

public class TerrainBuilder
{
    EcsWorld _world;

    private EcsEntity _terrainEntity;

    public TerrainBuilder(EcsWorld world)
    {
        _world = world;
    }

    public TerrainBuilder Create()
    {
        _terrainEntity = _world.NewEntity();
        return this;
    }

    public TerrainBuilder WithCode(string code)
    {
        _terrainEntity.Replace(new TerrainCode(code));
        return this;
    }

    public TerrainBuilder WithTypes(List<TerrainType> types)
    {
        _terrainEntity.Replace(new TerrainTypes(types));
        return this;
    }

    public EcsEntity Build()
    {
        return _terrainEntity;
    }
}
