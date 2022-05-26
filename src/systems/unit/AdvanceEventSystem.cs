using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;
using Godot;

public class AdvanceEvent
{
    public Entity Entity { get; set; }

    public AdvanceEvent(Entity entity)
    {
        Entity = entity;
    }
}

public class AdvanceEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<UnitPanel>(out var unitPanel))
        {
            return;
        }

        commands.Receive((AdvanceEvent advanceEvent) =>
        {
            var unitEntity = advanceEvent.Entity;

            var health = unitEntity.Get<Attribute<Health>>();
            var experience = unitEntity.Get<Attribute<Experience>>();
            var oldMoves = unitEntity.Get<Attribute<Moves>>();
            var oldActions = unitEntity.Get<Attribute<Actions>>();

            health.Restore();
            experience.Empty();

            var remainingMoves = oldMoves.Value;
            var remainingActions = oldActions.Value;

            var advancements = unitEntity.Get<Advancements>();

            if (advancements.List.Count == 0)
            {
                return;
            }

            var unitTypeId = advancements.List[0];
            var unitType = Data.Instance.Units[unitTypeId].Instantiate<UnitType>();

            var unitView = unitEntity.Get<UnitView>();
            var position = unitView.Position;
            var parent = unitView.GetParent();

            parent.AddChild(unitType);
            unitView = unitType.UnitView;
            unitType.RemoveChild(unitView);
            parent.AddChild(unitView);
            UnitFactory.CreateFromUnitType(commands, unitType, unitView, unitEntity);

            unitType.QueueFree();

            var coords = unitEntity.Get<Coords>();
            var level = unitEntity.Get<Level>();

            var moves = unitEntity.Get<Attribute<Moves>>();
            var actions = unitEntity.Get<Attribute<Actions>>();

            moves.Value = remainingMoves;
            actions.Value = remainingActions;

            unitView.Position = position;

            commands.Send(new SpawnFloatingLabelEvent(coords.World() + Vector3.Up * 8f, $"++{level.Value}++", new Color(1f, 1f, 0.6f)));
        });
    }
}