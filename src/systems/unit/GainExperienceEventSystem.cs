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
        var query = world.Query<GainExperienceEvent>().End();

        foreach (var id in query)
        {
            var hudView = world.GetResource<HudView>();

            ref var gainEvent = ref world.Entity(id).Get<GainExperienceEvent>();

            var entity = gainEvent.Entity;
            var amount = gainEvent.Amount;

            ref var experience = ref entity.Get<Attribute<Experience>>();

            experience.Increase(amount);

            ref var coords = ref entity.Get<Coords>();
            hudView.SpawnFloatingLabel(coords.World() + Vector3.Up * 7f, $"XP + {amount}", new Color(0.8f, 0.8f, 1f));

            if (experience.IsFull())
            {
                world.Spawn().Add(new AdvanceEvent(entity));
                experience.Empty();
            }
        }
    }
}