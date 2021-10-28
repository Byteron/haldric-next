public struct Health { } // For Attribute<T>

public struct Experience { } // For Attribute<T>

public struct Moves { } // For Attribute<T>

public struct Actions { } // For Attribute<T>

public struct Attribute<T>
{
    public int Value { get; set; }
    public int Max { get; set; }

    public Attribute(int max)
    {
        Max = max;
        Value = Max;
    }

    public int GetDifference()
    {
        return Max - Value;
    }

    public void Increase(int amount)
    {
        int sum = Value + amount;
        Value = sum > Max ? Max : sum;
    }

    public void Decrease(int amount)
    {
        int diff = Value - amount;
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