using Haldric.Wdk;

public struct Damage
{
    public int Value;
    public DamageType Type;

    public Damage(int value, DamageType type)
    {
        Value = value;
        Type = type;
    }
}