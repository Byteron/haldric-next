using RelEcs;
using RelEcs.Godot;

public struct Elevation : IReset<Elevation>
{
    public int Value { get; set; }
    public float Height { get { return Value * Metrics.ElevationStep; } }

    public Elevation(int value, int offset = 0)
    {
        Value = value;
    }

    public void Reset(ref Elevation c)
    {
        c.Value = 0;
    }
}

public struct ElevationOffset : IReset<ElevationOffset>
{
    public float Value { get; set; }

    public ElevationOffset(float value)
    {
        Value = value;
    }

    public void Reset(ref ElevationOffset c)
    {
        c.Value = 0f;
    }
}