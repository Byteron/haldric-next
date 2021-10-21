using System.Collections.Generic;
using Godot;
using Bitron.Ecs;

public class TerrainDictBuilder
{
    private Dictionary<string, object> _terrainDict;

    public TerrainDictBuilder CreateBase()
    {
        _terrainDict = new Dictionary<string, object> ();
        _terrainDict["BaseTerrain"] = true;
        return this;
    }
    public TerrainDictBuilder CreateOverlay()
    {
        _terrainDict = new Dictionary<string, object> ();
        _terrainDict["OverlayTerrain"] = true;
        return this;
    }

    public TerrainDictBuilder WithCode(string code)
    {
        _terrainDict["TerrainCode"] = code;
        return this;
    }

    public TerrainDictBuilder WithTypes(List<TerrainType> types)
    {
        _terrainDict["TerrainTypes"] = types;
        return this;
    }

    public TerrainDictBuilder WithHasWater()
    {
        _terrainDict["HasWater"] = true;
        return this;
    }

    public TerrainDictBuilder WithRecruitFrom()
    {
        _terrainDict["RecruitFrom"] = true;
        return this;
    }

    public TerrainDictBuilder WithRecruitTo()
    {
        _terrainDict["RecruitTo"] = true;
        return this;
    }

    public TerrainDictBuilder WithGivesIncome()
    {
        _terrainDict["GivesIncome"] = true;
        return this;
    }

    public TerrainDictBuilder WithIsCapturable()
    {
        _terrainDict["IsCapturable"] = true;
        return this;
    }

    public Dictionary<string, object>  Build()
    {
        return _terrainDict;
    }
}
