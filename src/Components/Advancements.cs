using System.Collections.Generic;

public class Advancements
{
    public List<string> List { get; }

    public Advancements() => List = new List<string>();
    public Advancements(List<string> unitTypeIds)
    {
        List = new List<string>();
        if (unitTypeIds != null)
        {
            List.AddRange(unitTypeIds);
        }
    }
}