using System.Collections.Generic;
using RelEcs;

public class Recruits
{
    public List<string> List;

    public Recruits() => List = new List<string>();
    public Recruits(List<string> unitTypeIds)
    {
        List = new List<string>();
        if (unitTypeIds != null)
        {
            List.AddRange(unitTypeIds);
        }
    }
}