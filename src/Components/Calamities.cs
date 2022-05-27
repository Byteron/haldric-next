using System.Collections.Generic;
using Haldric.Wdk;
using RelEcs;
using RelEcs.Godot;

public class Calamities
{
    public List<DamageType> List { get; }
    
    public Calamities() => List = new List<DamageType>();
    public Calamities(List<DamageType> types)
    {
        List = new List<DamageType>();
        if (types != null)
        {
            List.AddRange(types);
        }
    }
}