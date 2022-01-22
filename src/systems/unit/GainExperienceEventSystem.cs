using Bitron.Ecs;
using Godot;

public struct GainExperienceEvent
{
    public EcsEntity Entity { get; set; }
    public int Amount { get; set; }

    public GainExperienceEvent(EcsEntity entity, int amount)
    {
        Entity = entity;
        Amount = amount;
    }
}

public class GainExperienceEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        world.ForEach((ref GainExperienceEvent gainEvent) =>
        {
            var entity = gainEvent.Entity;
            var amount = gainEvent.Amount;

            ref var experience = ref entity.Get<Attribute<Experience>>();

            experience.Increase(amount);

            ref var coords = ref entity.Get<Coords>();

            world.Spawn().Add(new SpawnFloatingLabelEvent(coords.World() + Vector3.Up * 7f, $"XP + {amount}", new Color(0.8f, 0.8f, 1f)));

            if (experience.IsFull())
            {
                world.Spawn().Add(new AdvanceEvent(entity));
                experience.Empty();
            }
        });
    }
}