using Bitron.Ecs;

public struct Elevation : IEcsAutoReset<Elevation>
{
    public int Value { get; set; }
    public int Offset { get; set; }

    public float ValueWithOffset { get { return Value + Offset; }}
    public float Height { get { return Value * Metrics.ElevationStep; }}
    public float HeightWithOffset { get { return (Value + Offset) * Metrics.ElevationStep; }}
    
    public Elevation(int value, int offset = 0)
    {
        Value = value;
        Offset = offset;
    }

    public void AutoReset(ref Elevation c)
    {
        c.Value = 0;
    }
}