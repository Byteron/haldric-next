using Godot;
using System.Collections.Generic;
using Bitron.Ecs;

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
        _terrainEntity = Main.Instance.World.Spawn();
        _terrainEntity.Add<BaseTerrain>();
        return this;
    }
    public TerrainBuilder CreateOverlay()
    {
        _terrainEntity = Main.Instance.World.Spawn();
        _terrainEntity.Add<OverlayTerrain>();
        return this;
    }

    public TerrainBuilder WithCode(string code)
    {
        _terrainEntity.Add(new TerrainCode(code));
        return this;
    }

    public TerrainBuilder WithTypes(List<TerrainType> types)
    {
        _terrainEntity.Add(new TerrainTypes(types));
        return this;
    }

    public TerrainBuilder WithHasWater()
    {
        _terrainEntity.Add<HasWater>();
        return this;
    }

    public TerrainBuilder WithRecruitFrom()
    {
        _terrainEntity.Add<RecruitFrom>();
        return this;
    }

    public TerrainBuilder WithRecruitTo()
    {
        _terrainEntity.Add<RecruitTo>();
        return this;
    }

    public EcsEntity Build()
    {
        return _terrainEntity;
    }
}
