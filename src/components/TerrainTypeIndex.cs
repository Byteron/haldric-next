using Bitron.Ecs;

public struct TerrainTypeIndex : IEcsAutoReset<TerrainTypeIndex>
{
    public int Value;

    public TerrainTypeIndex(int value)
    {
        Value = value;
    }

    public void AutoReset(ref TerrainTypeIndex c)
    {
        c.Value = 0;
    }
}