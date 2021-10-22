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

    public EcsEntity GetFirst()
    {
        return _list[0];
    }

    public bool HasAttackWithRange(int range)
    {
        foreach(var attack in _list)
        {
            if (attack.Get<Range>().Value >= range)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasUsableAttack(int actions, int range)
    {
        foreach(var attack in _list)
        {
            if (attack.Get<Range>().Value >= range && attack.Get<Costs>().Value <= actions)
            {
                return true;
            }
        }
        return false;
    }

    public EcsEntity GetUsableAttack(int actions, int range)
    {
        foreach(var attack in _list)
        {
            if (attack.Get<Range>().Value >= range && attack.Get<Costs>().Value <= actions)
            {
                return attack;
            }
        }
        return default;
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