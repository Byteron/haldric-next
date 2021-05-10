using Godot;
using System.Collections.Generic;
using Leopotam.Ecs;

public struct Locations : IEcsAutoReset<Locations>
{
    Dictionary<Vector3, EcsEntity> _dict;

    public Dictionary<Vector3, EcsEntity> Dict { get { return _dict; } }

    public Dictionary<Vector3, EcsEntity>.ValueCollection Values { get { return _dict.Values; } }

    public int Count { get { return _dict.Count; } }

    public bool Has(Vector3 cell)
    {
        return _dict.ContainsKey(cell);
    }

    public EcsEntity Get(Vector3 cell)
    {
        if (_dict.ContainsKey(cell))
        {
            return _dict[cell];
        }

        return EcsEntity.Null;
    }

    public void Set(Vector3 cell, EcsEntity entity)
    {
        if (_dict == null)
        {
            _dict = new Dictionary<Vector3, EcsEntity>();
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
        c._dict = new Dictionary<Vector3, EcsEntity>();
    }
}