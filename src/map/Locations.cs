using Godot;
using System.Collections.Generic;
using Leopotam.Ecs;

public struct Locations : IEcsAutoReset<Locations>
{
    Dictionary<Vector3, EcsEntity> Dict;

    public void AutoReset(ref Locations c)
    {
        c.Dict = new Dictionary<Vector3, EcsEntity>();
    }

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

        return EcsEntity.Null;
    }

    public void Set(Vector3 cell, EcsEntity entity)
    {
        Dict.Add(cell, entity);
    }
}