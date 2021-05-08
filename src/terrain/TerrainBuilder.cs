using Godot;
using System.Collections.Generic;
using Leopotam.Ecs;


struct RecruitFrom {}
struct RecruitTo {}

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

    public TerrainBuilder WithRecruitFrom()
    {
        _terrainEntity.Get<RecruitFrom>();
        return this;
    }

    public TerrainBuilder WithRecruitTo()
    {
        _terrainEntity.Get<RecruitTo>();
        return this;
    }

    public EcsEntity Build()
    {
        return _terrainEntity;
    }
}
