using Haldric.Wdk;

public struct Damage
{
    public int Value { get; set; }
    public DamageType Type { get; set; }

    public Damage(int value, DamageType type)
    {
        Value = value;
        Type = type;
    }
}