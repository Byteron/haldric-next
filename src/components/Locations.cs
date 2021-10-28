using Godot;
using System.Collections.Generic;
using Bitron.Ecs;
using System;

public struct Locations : IEcsAutoReset<Locations>
{
    public Dictionary<Vector3, EcsEntity> Dict { get; set; }

    public Dictionary<Vector3, EcsEntity>.ValueCollection Values { get { return Dict.Values; } }
    public int Count { get { return Dict.Count; } }

    public bool Has(Vector3 cell)
    {
        return Dict.ContainsKey(cell);
    }

    public EcsEntity Get(Vector3 cell)
    {
        if (Dict.ContainsKey(cell))
        {
            return Dict[cell];
        }

        return default;
    }

    public void Set(Vector3 cell, EcsEntity entity)
    {
        if (Dict == null)
        {
            Dict = new Dictionary<Vector3, EcsEntity>();
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

    public void AutoReset(ref Locations c)
    {
        if (c.Dict != null)
        {
            c.Dict.Clear();
        }
        else
        {
            c.Dict = new Dictionary<Vector3, EcsEntity>();
        }
    }
}