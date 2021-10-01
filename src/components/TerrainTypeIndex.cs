using Bitron.Ecs;

public struct TerrainTypeIndex : IEcsAutoReset<TerrainTypeIndex>
{
    public uint Value;

    public TerrainTypeIndex(uint value)
    {
        Value = value;
    }

    public void AutoReset(ref TerrainTypeIndex c)
    {
        c.Value = 0;
    }
}