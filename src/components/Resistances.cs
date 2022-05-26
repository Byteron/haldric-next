using System.Collections.Generic;
using Haldric.Wdk;
using RelEcs;
using RelEcs.Godot;

public class Resistances
{
    public List<DamageType> List { get; }

    public Resistances() => List = new List<DamageType>();
    public Resistances(List<DamageType> types)
    {
        List = new List<DamageType>();
        if (types != null)
        {
            List.AddRange(types);
        }
    }
}