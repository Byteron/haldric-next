using System.Collections.Generic;
using Bitron.Ecs;
using Godot;

public struct CombatEvent
{
    public EcsEntity AttackerEntity;
    public EcsEntity DefenderEntity;

    public CombatEvent(EcsEntity attackerEntity, EcsEntity defenderEntity)
    {
        AttackerEntity = attackerEntity;
        DefenderEntity = defenderEntity;
    }
}

public class CombatEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<CombatEvent>().End();

        foreach (var eventEntityId in query)
        {
            ref var combatEvent = ref query.Get<CombatEvent>(eventEntityId);

            GD.Print($"Combat Event: {combatEvent.AttackerEntity.Get<Id>().Value} vs {combatEvent.DefenderEntity.Get<Id>().Value}");

            var attackerEntity = combatEvent.AttackerEntity;
            var defenderEntity = combatEvent.DefenderEntity;

            var attackerAttackEntity = attackerEntity.Get<Attacks>().GetFirst();
            var defenderAttackEntity = defenderEntity.Get<Attacks>().GetFirst();

            var attackerStrikes = attackerAttackEntity.Get<Strikes>().Value;
            var defenderStrikes = defenderAttackEntity.Get<Strikes>().Value;
            
            var queue = new Queue<DamageEvent>();

            for (int i = 0; i < Godot.Mathf.Max(attackerStrikes, defenderStrikes); i++)
            {
                if (i < attackerStrikes)
                {
                    queue.Enqueue(new DamageEvent(attackerAttackEntity, defenderEntity));
                }

                if (i < defenderStrikes)
                {
                    queue.Enqueue(new DamageEvent(defenderAttackEntity, attackerEntity));
                }
            }

            while (queue.Count > 0)
            {
                world.Spawn().Add(queue.Dequeue());
            }
        }
    }
}