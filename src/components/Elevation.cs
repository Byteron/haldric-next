using Bitron.Ecs;

public struct Elevation : IEcsAutoReset<Elevation>
{
    public int Value;

    public float Height { get { return Value * Metrics.ElevationStep; }}
    
    public Elevation(int value)
    {
        Value = value;
    }

    public void AutoReset(ref Elevation c)
    {
        c.Value = 0;
    }
}