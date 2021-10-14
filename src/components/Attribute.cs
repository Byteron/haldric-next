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
        Godot.GD.Print("Attribute Restored " + typeof(T).ToString());
    }

    public override string ToString()
    {
        return $"({Value} / {Max})";
    }
}