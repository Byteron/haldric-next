using RelEcs;
using RelEcs.Godot;

public struct Name : IReset<Name>
{
    public string Value { get; set; }

    public Name(string value)
    {
        Value = value;
    }

    public void Reset(ref Name c)
    {
        c.Value = "";
    }
}