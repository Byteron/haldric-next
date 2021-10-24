using System.Collections.Generic;
using Haldric.Wdk;

public struct TerrainTypes
{
    List<TerrainType> Value;

    public TerrainTypes(List<TerrainType> types)
    {
        Value = types;
    }
}