using Godot;
using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using System;

public class Locations
{
    public Dictionary<Vector3, Entity> Dict { get; set; } = new();

    public Dictionary<Vector3, Entity>.ValueCollection Values => Dict.Values;

    public int Count => Dict.Count;

    public bool Has(Vector3 cell)
    {
        return Dict.ContainsKey(cell);
    }

    public Entity Get(Vector3 cell)
    {
        return Dict.ContainsKey(cell) ? Dict[cell] : default;
    }

    public void Set(Vector3 cell, Entity entity)
    {
        Dict ??= new Dictionary<Vector3, Entity>();

        if (Dict.ContainsKey(cell))
        {
            Dict[cell] = entity;
        }
        else
        {
            Dict.Add(cell, entity);
        }
    }
}