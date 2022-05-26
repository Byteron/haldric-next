using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;

public class DamageEvent
{
    public Entity DamagingEntity { get; set; }
    public Entity TargetEntity { get; set; }
    public Alignment Alignment { get; set; }

    public DamageEvent(Entity damagingEntity, Entity targetEntity, Alignment alignment)
    {
        DamagingEntity = damagingEntity;
        TargetEntity = targetEntity;
        Alignment = alignment;
    }
}

public class DamageEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<Schedule>(out var schedule)) return;

        var daytime = schedule.GetCurrentDaytime();

        commands.Receive((DamageEvent damageEvent) =>
        {
            var damagingEntity = damageEvent.DamagingEntity;
            var targetEntity = damageEvent.TargetEntity;

            var damage = damagingEntity.Get<Damage>();

            var modifier = 1.0f * daytime.GetDamageModifier(damageEvent.Alignment);

            if (targetEntity.Has<Weaknesses>())
            {
                var weaknesses = targetEntity.Get<Weaknesses>();

                if (weaknesses.List.Contains(damage.Type))
                {
                    modifier *= Modifiers.Weakness;
                }
            }

            if (targetEntity.Has<Resistances>())
            {
                var resistances = targetEntity.Get<Resistances>();

                if (resistances.List.Contains(damage.Type))
                {
                    modifier *= Modifiers.Resistance;
                }
            }

            if (targetEntity.Has<Calamities>())
            {
                var calamities = targetEntity.Get<Calamities>();

                if (calamities.List.Contains(damage.Type))
                {
                    modifier *= Modifiers.Calamity;
                }
            }

            if (targetEntity.Has<Immunities>())
            {
                var immunities = targetEntity.Get<Immunities>();

                if (immunities.List.Contains(damage.Type))
                {
                    modifier *= Modifiers.Immunity;
                }
            }

            var health = targetEntity.Get<Attribute<Health>>();

            var finalDamage = (int)(damage.Value * modifier);

            health.Decrease(finalDamage);

            var coords = damageEvent.TargetEntity.Get<Coords>();
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