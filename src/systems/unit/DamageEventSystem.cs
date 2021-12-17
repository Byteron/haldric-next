using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public struct DamageEvent
{
    public EcsEntity DamagerEntity { get; set; }
    public EcsEntity TargetEntity { get; set; }
    public Alignment Alignment { get; set; }

    public DamageEvent(EcsEntity damagerEntity, EcsEntity targetEntity, Alignment alignment)
    {
        DamagerEntity = damagerEntity;
        TargetEntity = targetEntity;
        Alignment = alignment;
    }
}

public class DamageEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {

        if (!world.TryGetResource<Schedule>(out var schedule))
        {
            return;
        }

        var query = world.Query<DamageEvent>().End();

        var daytime = schedule.GetCurrentDaytime();

        foreach (var eventEntityId in query)
        {
            ref var damageEvent = ref world.Entity(eventEntityId).Get<DamageEvent>();

            var damagerEntity = damageEvent.DamagerEntity;
            var targetEntity = damageEvent.TargetEntity;

            ref var damage = ref damagerEntity.Get<Damage>();

            var modifier = 1.0f * daytime.GetDamageModifier(damageEvent.Alignment);

            if (targetEntity.Has<Weaknesses>())
            {
                ref var weaknesses = ref targetEntity.Get<Weaknesses>();

                if (weaknesses.List.Contains(damage.Type))
                {
                    modifier *= Modifiers.Weakness;
                }
            }

            if (targetEntity.Has<Resistances>())
            {
                ref var resistances = ref targetEntity.Get<Resistances>();

                if (resistances.List.Contains(damage.Type))
                {
                    modifier *= Modifiers.Resistance;
                }
            }

            if (targetEntity.Has<Calamities>())
            {
                ref var calamities = ref targetEntity.Get<Calamities>();

                if (calamities.List.Contains(damage.Type))
                {
                    modifier *= Modifiers.Calamity;
                }
            }

            if (targetEntity.Has<Immunities>())
            {
                ref var immunities = ref targetEntity.Get<Immunities>();

                if (immunities.List.Contains(damage.Type))
                {
                    modifier *= Modifiers.Immunity;
                }
            }

            ref var health = ref targetEntity.Get<Attribute<Health>>();

            var finalDamage = (int)(damage.Value * modifier);

            health.Decrease(finalDamage);

            ref var coords = ref damageEvent.TargetEntity.Get<Coords>();
            var position = coords.World() + Vector3.Up * 5f;
            var text = finalDamage.ToString();
            var color = new Color(1f, 0f, 0f);
            var spawnLabelEvent = new SpawnFloatingLabelEvent(position, text, color);

            world.Spawn().Add(spawnLabelEvent);

            if (health.IsEmpty())
            {
                GD.Print("Death Event Spawned");
                world.Spawn().Add(new DeathEvent(targetEntity));
            }
        }
    }
}