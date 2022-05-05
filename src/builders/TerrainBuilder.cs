using Godot;
using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public class TerrainBuilder
{
    Commands commands;

    Entity terrainEntity;

    public TerrainBuilder(Commands commands)
    {
        this.commands = commands;
    }

    public TerrainBuilder CreateBase()
    {
        terrainEntity = commands.Spawn();
        terrainEntity.Add<IsBaseTerrain>();
        terrainEntity.Add<ElevationOffset>();
        return this;
    }
    public TerrainBuilder CreateOverlay()
    {
        terrainEntity = commands.Spawn();
        terrainEntity.Add<IsOverlayTerrain>();
        return this;
    }

    public TerrainBuilder WithCode(string code)
    {
        terrainEntity.Add(new TerrainCode(code));
        return this;
    }

    public TerrainBuilder WithTypes(List<TerrainType> types)
    {
        terrainEntity.Add(new TerrainTypes(types));
        return this;
    }

    public TerrainBuilder WithElevationOffset(float elevationOffset)
    {
        terrainEntity.Get<ElevationOffset>().Value = elevationOffset;
        return this;
    }

    public TerrainBuilder WithRecruitFrom()
    {
        terrainEntity.Add<CanRecruitFrom>();
        return this;
    }

    public TerrainBuilder WithRecruitTo()
    {
        terrainEntity.Add<CanRecruitTo>();
        return this;
    }

    public TerrainBuilder WithGivesIncome()
    {
        terrainEntity.Add<GivesIncome>();
        return this;
    }

    public TerrainBuilder WithIsCapturable()
    {
        terrainEntity.Add<IsCapturable>();
        return this;
    }

    public TerrainBuilder WithHeals()
    {
        terrainEntity.Add<Heals>();
        return this;
    }

    public TerrainBuilder WithNoLighting()
    {
        terrainEntity.Add<NoLighting>();
        return this;
    }

    public Entity Build()
    {
        return terrainEntity;
    }
}
