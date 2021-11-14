using Bitron.Ecs;

public struct Elevation : IEcsAutoReset<Elevation>
{
    public int Value { get; set; }
    public float Height { get { return Value * Metrics.ElevationStep; }}
    
    public Elevation(int value, int offset = 0)
    {
        Value = value;
    }

    public void AutoReset(ref Elevation c)
    {
        c.Value = 0;
    }
}

public struct ElevationOffset : IEcsAutoReset<ElevationOffset>
{
    public float Value { get; set; }
    
    public ElevationOffset(float value)
    {
        Value = value;
    }

    public void AutoReset(ref ElevationOffset c)
    {
        c.Value = 0f;
    }
}