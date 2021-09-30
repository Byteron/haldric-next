using Godot;
using System.Collections.Generic;
using Bitron.Ecs;

public class TerrainBuilder
{
    private EcsEntity _terrainEntity;

    public TerrainBuilder CreateBase()
    {
        _terrainEntity = Main.Instance.World.Spawn();
        _terrainEntity.Add<IsBaseTerrain>();
        return this;
    }
    public TerrainBuilder CreateOverlay()
    {
        _terrainEntity = Main.Instance.World.Spawn();
        _terrainEntity.Add<IsOverlayTerrain>();
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
        _terrainEntity.Add<CanRecruitFrom>();
        return this;
    }

    public TerrainBuilder WithRecruitTo()
    {
        _terrainEntity.Add<CanRecruitTo>();
        return this;
    }

    public EcsEntity Build()
    {
        return _terrainEntity;
    }
}
