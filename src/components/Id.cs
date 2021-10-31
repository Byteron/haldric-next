using Bitron.Ecs;

public struct Id : IEcsAutoReset<Id>
{
    public string Value { get; set; }

    public Id(string value)
    {
        Value = value;
    }

    public void AutoReset(ref Id c)
    {
        c.Value = "";
    }
}