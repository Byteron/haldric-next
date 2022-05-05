using RelEcs;
using RelEcs.Godot;

public struct Index : IReset<Index>
{
    public int Value { get; set; }

    public Index(int value)
    {
        Value = value;
    }

    public void Reset(ref Index c)
    {
        c.Value = -1;
    }
}