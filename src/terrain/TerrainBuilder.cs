using Godot;
using System.Collections.Generic;

public class TerrainBuilder
{
    private TerrainData _terrainData = new TerrainData();

    public TerrainBuilder Create()
    {
        _terrainData = new TerrainData();
        return this;
    }

    public TerrainBuilder WithCode(string code)
    {
        _terrainData.Code = code;
        return this;
    }

    public TerrainBuilder WithName(string name)
    {
        _terrainData.Name = name;
        return this;
    }

    public TerrainBuilder WithTypes(List<TerrainType> types)
    {
        _terrainData.Types = types;
        return this;
    }

    public TerrainData Build()
    {
        return _terrainData;
    }
}
