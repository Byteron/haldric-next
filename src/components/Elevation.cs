using Bitron.Ecs;

public struct Elevation : IEcsAutoReset<Elevation>
{
    public int Level;

    public float Height { get { return Level * Metrics.ElevationStep; }}
    
    public Elevation(int level)
    {
        Level = level;
    }

    public void AutoReset(ref Elevation c)
    {
        c.Level = 0;
    }
}