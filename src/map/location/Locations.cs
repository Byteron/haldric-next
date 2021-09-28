using Godot;
using System.Collections.Generic;
using Bitron.Ecs;

public struct Locations : IEcsAutoReset<Locations>
{
    Dictionary<Vector3, EcsPackedEntity> _dict;

    public Dictionary<Vector3, EcsPackedEntity> Dict { get { return _dict; } }

    public Dictionary<Vector3, EcsPackedEntity>.ValueCollection Values { get { return _dict.Values; } }

    public int Count { get { return _dict.Count; } }

    public bool Has(Vector3 cell)
    {
        return _dict.ContainsKey(cell);
    }

    public EcsPackedEntity Get(Vector3 cell)
    {
        if (_dict.ContainsKey(cell))
        {
            return _dict[cell];
        }

        return default;
    }

    public void Set(Vector3 cell, EcsPackedEntity entity)
    {
        if (_dict == null)
        {
            _dict = new Dictionary<Vector3, EcsPackedEntity>();
        }

        if (_dict.ContainsKey(cell))
        {
            _dict[cell] = entity;
        }
        else
        {
            _dict.Add(cell, entity);
        }
    }

    public void Clear()
    {
        _dict.Clear();
    }

    public void AutoReset(ref Locations c)
    {
        c._dict = new Dictionary<Vector3, EcsPackedEntity>();
    }
}