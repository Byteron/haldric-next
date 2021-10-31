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

    public EcsEntity GetUsableAttack(int attackRange, int bonusAttackRange = 0)
    {
        bool isInMeleeRange = attackRange == 1;

        foreach(var attack in _list)
        {
            if (isInMeleeRange)
            {
                if (attack.Get<Range>().Value == 1)
                {
                    return attack;
                }
            }
            else
            {
                if (attack.Get<Range>().Value + bonusAttackRange >= attackRange)
                {
                    return attack;
                }
            }
        }
        return default;
    }

    public EcsEntity[] GetUsableAttacks(int attackDistance, int bonusAttackRange = 0)
    {
        List<EcsEntity> list = new List<EcsEntity>();

        bool isInMeleeRange = attackDistance == 1;
        
        foreach(var attack in _list)
        {
            if (isInMeleeRange)
            {
                if (attack.Get<Range>().Value == 1)
                {
                    list.Add(attack);
                }
            }
            else
            {
                if (attack.Get<Range>().Value + bonusAttackRange >= attackDistance)
                {
                    list.Add(attack);
                }
            }
        }
        return list.ToArray();
    }

    public int GetMaxAttackRange()
    {
        var range = 0;

        foreach(var attack in _list)
        {
            var attackRange = attack.Get<Range>().Value;

            if (attackRange > range)
            {
                range = attackRange;
            }
        }

        return range;
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