using System.Collections.Generic;

public class TerrainDictBuilder
{
     Dictionary<string, object> _terrainDict;

    public TerrainDictBuilder CreateBase()
    {
        _terrainDict = new Dictionary<string, object>
        {
            [nameof(IsBaseTerrain)] = true
        };
        return this;
    }
    public TerrainDictBuilder CreateOverlay()
    {
        _terrainDict = new Dictionary<string, object>
        {
            [nameof(IsOverlayTerrain)] = true
        };
        return this;
    }

    public TerrainDictBuilder WithCode(string code)
    {
        _terrainDict[nameof(TerrainCode)] = code;
        return this;
    }

    public TerrainDictBuilder WithTypes(List<TerrainType> types)
    {
        _terrainDict[nameof(TerrainTypes)] = types;
        return this;
    }

    public TerrainDictBuilder WithElevationOffset(float elevationOffset)
    {
        _terrainDict[nameof(ElevationOffset)] = elevationOffset;
        return this;
    }

    public TerrainDictBuilder WithRecruitFrom()
    {
        _terrainDict[nameof(CanRecruitFrom)] = true;
        return this;
    }

    public TerrainDictBuilder WithRecruitTo()
    {
        _terrainDict[nameof(CanRecruitTo)] = true;
        return this;
    }

    public TerrainDictBuilder WithGivesIncome()
    {
        _terrainDict[nameof(GivesIncome)] = true;
        return this;
    }

    public TerrainDictBuilder WithIsCapturable()
    {
        _terrainDict[nameof(IsCapturable)] = true;
        return this;
    }

    public TerrainDictBuilder WithHeals()
    {
        _terrainDict[nameof(Heals)] = true;
        return this;
    }

    public TerrainDictBuilder NoLighting()
    {
        _terrainDict[nameof(NoLighting)] = true;
        return this;
    }

    public Dictionary<string, object> Build()
    {
        return _terrainDict;
    }
}
