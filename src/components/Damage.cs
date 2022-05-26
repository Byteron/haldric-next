using Haldric.Wdk;

public class Damage
{
    public int Value { get; set; }
    public DamageType Type { get; set; }

    public Damage()
    {
    }
    
    public Damage(int value, DamageType type)
    {
        Value = value;
        Type = type;
    }
}