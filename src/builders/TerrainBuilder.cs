using Godot;
using System.Collections.Generic;
using Bitron.Ecs;
using Haldric.Wdk;

public class TerrainBuilder
{
    private EcsEntity _terrainEntity;

    public TerrainBuilder CreateBase()
    {
        _terrainEntity = Main.Instance.World.Spawn();
        _terrainEntity.Add<IsBaseTerrain>();
        _terrainEntity.Add<ElevationOffset>();
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

    public TerrainBuilder WithElevationOffset(float elevationOffset)
    {
        _terrainEntity.Get<ElevationOffset>().Value = elevationOffset;
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

    public TerrainBuilder WithGivesIncome()
    {
        _terrainEntity.Add<GivesIncome>();
        return this;
    }

    public TerrainBuilder WithIsCapturable()
    {
        _terrainEntity.Add<IsCapturable>();
        return this;
    }

    public TerrainBuilder WithHeals()
    {
        _terrainEntity.Add<Heals>();
        return this;
    }

    public EcsEntity Build()
    {
        return _terrainEntity;
    }
}
