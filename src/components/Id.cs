using RelEcs;
using RelEcs.Godot;

public struct Id : IReset<Id>
{
    public string Value { get; set; }

    public Id(string value)
    {
        Value = value;
    }

    public void Reset(ref Id c)
    {
        c.Value = "";
    }
}