using System.Collections.Generic;
using Bitron.Ecs;

public struct Attacks : IEcsAutoReset<Attacks>
{
    List<EcsEntity> _list;

    public void Add(EcsEntity attackEntity)
    {
        if (_list == null)
        {
            _list = new List<EcsEntity>();
        }

        _list.Add(attackEntity);
    }

    public List<EcsEntity> GetList()
    {
        return _list;
    }

    public void AutoReset(ref Attacks c)
    {
        if (c._list == null)
        {
            c._list = new List<EcsEntity>();
        }
        else
        {
            c._list.Clear();
        }
    }
}