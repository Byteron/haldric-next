using System.Collections.Generic;
using RelEcs;

public class Attacks
{
    public List<Entity> List = new();

    public void Add(Entity attackEntity)
    {
        if (List == null)
        {
            List = new List<Entity>();
        }

        List.Add(attackEntity);
    }

    // public Entity GetAttack(string id)
    // {
    //     foreach (var attack in List)
    //     {
    //         if (attack.Get<Id>().Value == id)
    //         {
    //             return attack;
    //         }
    //     }

    //     return default;
    // }

    // public Entity GetUsableAttack(bool isInMeleeRange, int attackRange)
    // {
    //     foreach (var attack in List)
    //     {
    //         if (isInMeleeRange)
    //         {
    //             if (attack.Get<Range>().Value == 1)
    //             {
    //                 return attack;
    //             }
    //         }
    //         else
    //         {
    //             if (attack.Get<Range>().Value == 1)
    //             {
    //                 continue;
    //             }

    //             if (attack.Get<Range>().Value >= attackRange)
    //             {
    //                 return attack;
    //             }
    //         }
    //     }
    //     return default;
    // }

    // public Entity[] GetUsableAttacks(bool isInMeleeRange, int attackDistance)
    // {
    //     List<Entity> list = new List<Entity>();

    //     foreach (var attack in List)
    //     {
    //         if (isInMeleeRange)
    //         {
    //             if (attack.Get<Range>().Value == 1)
    //             {
    //                 list.Add(attack);
    //             }
    //         }
    //         else
    //         {
    //             if (attack.Get<Range>().Value == 1)
    //             {
    //                 continue;
    //             }

    //             if (attack.Get<Range>().Value >= attackDistance)
    //             {
    //                 list.Add(attack);
    //             }
    //         }
    //     }
    //     return list.ToArray();
    // }

    // public int GetMaxAttackRange()
    // {
    //     var range = 0;

    //     foreach (var attack in List)
    //     {
    //         var attackRange = attack.Get<Range>().Value;

    //         if (attackRange > range)
    //         {
    //             range = attackRange;
    //         }
    //     }

    //     return range;
    // }
}