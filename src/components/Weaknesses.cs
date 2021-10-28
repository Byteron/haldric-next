using System.Collections.Generic;
using Haldric.Wdk;
using Bitron.Ecs;

public struct Weaknesses: IEcsAutoReset<Weaknesses>
{
    public List<DamageType> List { get; set; }

    public Weaknesses(List<DamageType> types)
    {
        List = new List<DamageType>();
        if (types != null)
        {
            List.AddRange(types);
        }
    }

    public void AutoReset(ref Weaknesses c)
    {
        if (c.List != null)
        {
            c.List.Clear();
        }
        else
        {
            c.List = new List<DamageType>();
        }
    }
}