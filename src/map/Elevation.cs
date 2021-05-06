using Leopotam.Ecs;

public struct Elevation : IEcsAutoReset<Elevation>
{
    public int Level;
    public float Step;

    public float Height { get { return Level * Step; }}
    
    public Elevation(int level, float step)
    {
        Level = level;
        Step = step;
    }

    public void AutoReset(ref Elevation c)
    {
        c.Level = 0;
        c.Step = 0f;
    }
}