using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;

public struct Recruits : IReset<Recruits>
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

    public void Reset(ref Recruits c)
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