using System.Collections.Generic;
using Bitron.Ecs;

public struct Recruits: IEcsAutoReset<Recruits>
{
    public List<string> List;

    public Recruits(List<string> unitTypeIds)
    {
        List = new List<string>();
        if (unitTypeIds != null)
        {
            List.AddRange(unitTypeIds);
        }
    }

    public void AutoReset(ref Recruits c)
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