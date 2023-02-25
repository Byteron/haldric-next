using System.Collections.Generic;

namespace Haldric;

public class Terrain
{
    public int Index;

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