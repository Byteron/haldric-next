using System.Collections.Generic;
using Haldric.Wdk;
using Bitron.Ecs;

public struct Calamities: IEcsAutoReset<Calamities>
{
    public List<DamageType> List;

    public Calamities(List<DamageType> list)
    {
        List = new List<DamageType>();
        List.AddRange(list);
    }

    public void AutoReset(ref Calamities c)
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