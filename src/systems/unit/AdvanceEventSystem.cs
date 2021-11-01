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
            var hudView = world.GetResource<HUDView>();
            
            ref var advanceEvent = ref world.Entity(id).Get<AdvanceEvent>();

            var entity = advanceEvent.Entity;

            ref var advancements = ref entity.Get<Advancements>();

            if (advancements.List.Count == 0)
            {
                continue;
            }

            var unitTypeId = advancements.List[0];
            var unitType = Data.Instance.Units[unitTypeId].Instantiate<UnitType>();

            var unitView = entity.Get<NodeHandle<UnitView>>().Node;
            var position = unitView.Position;
            var parent = unitView.GetParent();

            parent.AddChild(unitType);
            unitView = unitType.UnitView;
            unitType.RemoveChild(unitView);
            parent.AddChild(unitView);
            UnitFactory.CreateFromUnitType(world, unitType, unitView, entity);
            
            unitType.QueueFree();
            
            ref var coords = ref entity.Get<Coords>();
            ref var level = ref entity.Get<Level>();

            unitView.Position = position;

            hudView.SpawnFloatingLabel(coords.World + Vector3.Up * 8f, $"++{level.Value}++", new Color(1f, 1f, 0.6f));
        }
    }
}