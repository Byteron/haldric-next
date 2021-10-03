using Bitron.Ecs;

public struct Id : IEcsAutoReset<Id>
{
    public string Value;

    public Id(string value)
    {
        Value = value;
    }

    public void AutoReset(ref Id c)
    {
        c.Value = "";
    }
}