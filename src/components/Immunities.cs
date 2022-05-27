using System.Collections.Generic;
using Haldric.Wdk;
using RelEcs;
using RelEcs.Godot;

public class Immunities
{
    public List<DamageType> List { get; }

    public Immunities() => List = new List<DamageType>();
    public Immunities(List<DamageType> types)
    {
        List = new List<DamageType>();
        if (types != null)
        {
            List.AddRange(types);
        }
    }
}