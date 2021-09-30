using System.Collections.Generic;

public enum TerrainType
{
    Flat,
	Forested,
	Rough,
	Rocky,
	Aqueous,
	Cavernous,
	Settled,
	Fortified,
}

public struct TerrainTypes
{
    List<TerrainType> Value;

    public TerrainTypes(List<TerrainType> types)
    {
        Value = types;
    }
}