using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;

public struct DamageEvent
{
    public Entity DamagerEntity { get; set; }
    public Entity TargetEntity { get; set; }
    public Alignment Alignment { get; set; }

    public DamageEvent(Entity damagerEntity, Entity targetEntity, Alignment alignment)
    {
        DamagerEntity = damagerEntity;
        TargetEntity = targetEntity;
        Alignment = alignment;
    }
}

public class DamageEventSystem : ISystem
{
    public void Run(Commands commands)
    {

        if (!commands.TryGetElement<Schedule>(out var schedule))
        {
            return;
        }

        var daytime = schedule.GetCurrentDaytime();

        commands.Receive((DamageEvent damageEvent) =>
        {
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

            commands.Send(spawnLabelEvent);

            if (health.IsEmpty())
            {
                GD.Print("Death Event Spawned");
                commands.Send(new DeathEvent(targetEntity));
            }
        });
    }
}