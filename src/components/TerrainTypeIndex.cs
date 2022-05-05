using RelEcs;
using RelEcs.Godot;

public struct TerrainTypeIndex : IReset<TerrainTypeIndex>
{
    public int Value { get; set; }

    public TerrainTypeIndex(int value)
    {
        Value = value;
    }

    public void Reset(ref TerrainTypeIndex c)
    {
        c.Value = -1;
    }
}