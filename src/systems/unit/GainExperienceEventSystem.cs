using RelEcs;
using RelEcs.Godot;
using Godot;

public class GainExperienceEvent
{
    public Entity Entity { get; set; }
    public int Amount { get; set; }

    public GainExperienceEvent(Entity entity, int amount)
    {
        Entity = entity;
        Amount = amount;
    }
}

public class GainExperienceEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((GainExperienceEvent gainEvent) =>
        {
            var entity = gainEvent.Entity;
            var amount = gainEvent.Amount;

            var experience = entity.Get<Attribute<Experience>>();

            experience.Increase(amount);

            var coords = entity.Get<Coords>();

            commands.Send(new SpawnFloatingLabelEvent(coords.World() + Vector3.Up * 7f, $"XP + {amount}", new Color(0.8f, 0.8f, 1f)));

            if (!experience.IsFull()) return;
            
            commands.Send(new AdvanceEvent(entity));
            experience.Empty();
        });
    }
}