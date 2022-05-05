using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;

public struct Village : IReset<Village>
{
    public List<Entity> List { get; set; }

    public void Reset(ref Village c)
    {
        c.List = new List<Entity>();
    }
}