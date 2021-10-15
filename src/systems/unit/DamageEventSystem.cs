using Bitron.Ecs;
using Godot;

public struct DamageEvent
{
    public EcsEntity DamagerEntity;
    public EcsEntity TargetEntity;

    public DamageEvent(EcsEntity damagerEntity, EcsEntity targetEntity)
    {
        DamagerEntity = damagerEntity;
        TargetEntity = targetEntity;
    }
}

public class DamageEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<DamageEvent>().End();

        foreach (var eventEntityId in query)
        {
            ref var damageEvent = ref query.Get<DamageEvent>(eventEntityId);

            var damagerEntity = damageEvent.DamagerEntity;
            var targetEntity = damageEvent.TargetEntity;

            ref var targetHealth = ref targetEntity.Get<Attribute<Health>>();
            var damage = damagerEntity.Get<Damage>().Value;

            targetHealth.Decrease(damage);

            GD.Print($"Damage Event: {damagerEntity.Get<Id>().Value} dealt {damage} damage to {targetEntity.Get<Id>().Value}");
        }
    }
}