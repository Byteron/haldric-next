using Godot;
using System.Collections.Generic;
using RelEcs;
using Haldric.Wdk;

public class TerrainBuilder
{
    Commands _commands;

    Entity _terrainEntity;

    public TerrainBuilder(Commands commands)
    {
        _commands = commands;
    }

    public TerrainBuilder CreateBase()
    {
        _terrainEntity = _commands.Spawn();
        _terrainEntity.Add<IsBaseTerrain>();
        _terrainEntity.Add(new ElevationOffset());
        return this;
    }
    public TerrainBuilder CreateOverlay()
    {
        _terrainEntity = _commands.Spawn();
        _terrainEntity.Add<IsOverlayTerrain>();
        return this;
    }

    public TerrainBuilder WithCode(string code)
    {
        _terrainEntity.Add(new TerrainCode { Value = code });
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

    public TerrainBuilder WithNoLighting()
    {
        _terrainEntity.Add<NoLighting>();
        return this;
    }

    public Entity Build()
    {
        return _terrainEntity;
    }
}
