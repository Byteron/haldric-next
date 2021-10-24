using System.Collections.Generic;
using Haldric.Wdk;
using Bitron.Ecs;

public struct Resistances: IEcsAutoReset<Resistances>
{
    public List<DamageType> List;

    public Resistances(List<DamageType> list)
    {
        List = new List<DamageType>();
        List.AddRange(list);
    }

    public void AutoReset(ref Resistances c)
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