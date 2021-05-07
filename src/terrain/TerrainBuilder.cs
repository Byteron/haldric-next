using Godot;
using System.Collections.Generic;

public class TerrainBuilder
{
    private Terrain _terrainData = new Terrain();

    public TerrainBuilder Create()
    {
        _terrainData = new Terrain();
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

    public Terrain Build()
    {
        return _terrainData;
    }
}
