using Bitron.Ecs;
using Haldric.Wdk;
using Godot;

public struct AdvanceEvent
{
    public EcsEntity Entity { get; set; }

    public AdvanceEvent(EcsEntity entity)
    {
        Entity = entity;
    }
}

public class AdvanceEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<AdvanceEvent>().End();

        foreach (var id in query)
        {
            var hudView = world.GetResource<HudView>();

            ref var advanceEvent = ref world.Entity(id).Get<AdvanceEvent>();

            var unitEntity = advanceEvent.Entity;

            ref var health = ref unitEntity.Get<Attribute<Health>>();
            ref var experience = ref unitEntity.Get<Attribute<Experience>>();
            ref var oldMoves = ref unitEntity.Get<Attribute<Moves>>();
            ref var oldActions = ref unitEntity.Get<Attribute<Actions>>();

            health.Restore();
            experience.Empty();

            var remainingMoves = oldMoves.Value;
            var remainingActions = oldActions.Value;

            ref var advancements = ref unitEntity.Get<Advancements>();

            if (advancements.List.Count == 0)
            {
                continue;
            }

            var unitTypeId = advancements.List[0];
            var unitType = Data.Instance.Units[unitTypeId].Instantiate<UnitType>();

            var unitView = unitEntity.Get<NodeHandle<UnitView>>().Node;
            var position = unitView.Position;
            var parent = unitView.GetParent();

            parent.AddChild(unitType);
            unitView = unitType.UnitView;
            unitType.RemoveChild(unitView);
            parent.AddChild(unitView);
            UnitFactory.CreateFromUnitType(world, unitType, unitView, unitEntity);

            unitType.QueueFree();

            ref var coords = ref unitEntity.Get<Coords>();
            ref var level = ref unitEntity.Get<Level>();

            ref var moves = ref unitEntity.Get<Attribute<Moves>>();
            ref var actions = ref unitEntity.Get<Attribute<Actions>>();

            moves.Value = remainingMoves;
            actions.Value = remainingActions;

            unitView.Position = position;

            hudView.SpawnFloatingLabel(coords.World() + Vector3.Up * 8f, $"++{level.Value}++", new Color(1f, 1f, 0.6f));
        }
    }
}