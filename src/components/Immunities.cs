using System.Collections.Generic;
using Haldric.Wdk;
using Bitron.Ecs;

public struct Immunities: IEcsAutoReset<Immunities>
{
    public List<DamageType> List;

    public Immunities(List<DamageType> list)
    {
        List = new List<DamageType>();
        List.AddRange(list);
    }

    public void AutoReset(ref Immunities c)
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