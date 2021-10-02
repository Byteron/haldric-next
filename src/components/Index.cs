using Bitron.Ecs;

public struct Index : IEcsAutoReset<Index>
{
    public int Value;

    public Index(int value)
    {
        Value = value;
    }

    public void AutoReset(ref Index c)
    {
        c.Value = -1;
    }
}