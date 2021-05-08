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

public struct TerrainData
{
    public string Name;
    public string Code;
    public List<TerrainType> Types;
}
