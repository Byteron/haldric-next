using Godot;
using System.Collections.Generic;
using Leopotam.Ecs;

struct BaseTerrain {}
struct OverlayTerrain {}
struct RecruitFrom {}
struct RecruitTo {}
struct HasWater {}

public class TerrainBuilder
{
    private EcsEntity _terrainEntity;

    public TerrainBuilder CreateBase()
    {
        _terrainEntity = Main.Instance.World.NewEntity();
        _terrainEntity.Get<BaseTerrain>();
        return this;
    }
    public TerrainBuilder CreateOverlay()
    {
        _terrainEntity = Main.Instance.World.NewEntity();
        _terrainEntity.Get<OverlayTerrain>();
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

    public TerrainBuilder WithHasWater()
    {
        _terrainEntity.Get<HasWater>();
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
