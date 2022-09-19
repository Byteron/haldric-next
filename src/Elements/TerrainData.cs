using System.Collections.Generic;
using RelEcs;

public class TerrainInfo
{
    public string Code;
    public List<TerrainType> Types;
    public bool IsBase;
    public string DefaultBase;
    public float ElevationOffset;
    public bool CanRecruitFrom;
    public bool CanRecruitTo;
    public bool GivesIncome;
    public bool IsCapturable;
    public bool Heals;
}

public class TerrainData
{
    public Dictionary<string, TerrainInfo> Terrains = new();
    public Dictionary<string, Entity> TerrainEntities = new();
    public Dictionary<string, string> DefaultOverlayBaseTerrains = new();
}