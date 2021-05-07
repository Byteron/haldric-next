using Godot;
using System;
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

public struct Terrain
{
    public string Name;
    public string Code;
    public List<TerrainType> Types;
}
