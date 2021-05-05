using Leopotam.Ecs;

public struct Elevation : IEcsAutoReset<Elevation>
{
    public int Value;

    public Elevation(int value)
    {
        Value = value;
    }

    public void AutoReset(ref Elevation c)
    {
        c.Value = 0;
    }
}

public struct ElevationStep : IEcsAutoReset<ElevationStep>
{
    public float Value;

    public ElevationStep(float value)
    {
        Value = value;
    }

    public void AutoReset(ref ElevationStep c)
    {
        c.Value = 0f;
    }
}