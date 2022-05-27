using System.Collections.Generic;
using Haldric.Wdk;
using RelEcs;
using RelEcs.Godot;

public class Weaknesses
{
    public List<DamageType> List { get; }

    public Weaknesses() => List = new List<DamageType>();
    public Weaknesses(List<DamageType> types)
    {
        List = new List<DamageType>();
        if (types != null)
        {
            List.AddRange(types);
        }
    }
}