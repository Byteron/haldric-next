using Godot;
using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using System;

public struct Locations : IReset<Locations>
{
    public Dictionary<Vector3, Entity> Dict { get; set; }

    public Dictionary<Vector3, Entity>.ValueCollection Values { get { return Dict.Values; } }
    public int Count { get { return Dict.Count; } }

    public bool Has(Vector3 cell)
    {
        return Dict.ContainsKey(cell);
    }

    public Entity Get(Vector3 cell)
    {
        if (Dict.ContainsKey(cell))
        {
            return Dict[cell];
        }

        return default;
    }

    public void Set(Vector3 cell, Entity entity)
    {
        if (Dict == null)
        {
            Dict = new Dictionary<Vector3, Entity>();
        }

        if (Dict.ContainsKey(cell))
        {
            Dict[cell] = entity;
        }
        else
        {
            Dict.Add(cell, entity);
        }
    }

    public void Reset(ref Locations c)
    {
        if (c.Dict != null)
        {
            c.Dict.Clear();
        }
        else
        {
            c.Dict = new Dictionary<Vector3, Entity>();
        }
    }
}