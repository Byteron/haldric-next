using System.Collections.Generic;
using Haldric.Wdk;
using Bitron.Ecs;

public struct Calamities: IEcsAutoReset<Calamities>
{
    public List<DamageType> List { get; set; }

    public Calamities(List<DamageType> types)
    {
        List = new List<DamageType>();
        if (types != null)
        {
            List.AddRange(types);
        }
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