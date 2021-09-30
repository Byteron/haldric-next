public struct Health { } // For Attribute<T>

public struct Experience { } // For Attribute<T>

public struct Moves { } // For Attribute<T>

public struct Attribute<T>
{
    public int Value;
    public int Max;

    public Attribute(int max)
    {
        Max = max;
        Value = Max;
    }

    public void Increase(int amount)
    {
        int sum = Value + amount;
        Value = sum > Max ? Max : sum;
    }
}