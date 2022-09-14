using RelEcs;

public class Elevation
{
    public int Value { get; set; }
    public float Height => Value * Metrics.ElevationStep;
}

public class ElevationOffset
{
    public float Value { get; set; }
    public ElevationOffset() => Value = 0f;
    public ElevationOffset(float value) => Value = value;
}