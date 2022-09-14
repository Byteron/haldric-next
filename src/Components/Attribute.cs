using Godot;

public class Health : Attribute { }

public class Experience : Attribute { }

public class Moves : Attribute { }

public class Actions : Attribute { }

public class Attribute
{
    public int Value { get; set; }
    public int Max { get; set; }

    public int GetDifference()
    {
        return Max - Value;
    }

    public void Increase(int amount)
    {
        var sum = Value + amount;
        Value = sum > Max ? Max : sum;
    }

    public void Decrease(int amount)
    {
        var diff = Value - amount;
        Value = diff < 0 ? 0 : diff;
    }

    public void Empty()
    {
        Value = 0;
    }

    public void Restore()
    {
        Value = Max;
    }

    public bool IsEmpty()
    {
        return Value == 0;
    }

    public bool IsFull()
    {
        return Value == Max;
    }

    public override string ToString()
    {
        return $"{Value} / {Max}";
    }
}