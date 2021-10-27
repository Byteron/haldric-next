using System.Collections.Generic;
using Haldric.Wdk;
using Bitron.Ecs;

public struct Advancements: IEcsAutoReset<Advancements>
{
    public List<string> List;

    public Advancements(List<string> unitTypeIds)
    {
        List = new List<string>();
        if (unitTypeIds != null)
        {
            List.AddRange(unitTypeIds);
        }
    }

    public void AutoReset(ref Advancements c)
    {
        if (c.List != null)
        {
            c.List.Clear();
        }
        else
        {
            c.List = new List<string>();
        }
    }
}