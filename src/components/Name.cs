using Bitron.Ecs;

public struct Name : IEcsAutoReset<Name>
{
    public string Value { get; set; }

    public Name(string value)
    {
        Value = value;
    }

    public void AutoReset(ref Name c)
    {
        c.Value = "";
    }
}