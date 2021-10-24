using System.Collections.Generic;
using Haldric.Wdk;
using Bitron.Ecs;

public struct Weaknesses: IEcsAutoReset<Weaknesses>
{
    public List<DamageType> List;

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