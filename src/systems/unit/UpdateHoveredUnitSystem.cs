using Godot;
using Bitron.Ecs;

public class UpdateHoveredUnitSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.TryGetResource<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        var hoveredLocEntity = hoveredLocation.Entity;

        if (!hoveredLocEntity.IsAlive())
        {
            return;
        }

        if (!hoveredLocEntity.Has<HasUnit>())
        {
            return;
        }

        var hoveredUnitEntity = hoveredLocEntity.Get<HasUnit>().Entity;

        world.Spawn().Add(new UnitHoveredEvent(hoveredUnitEntity));
    }
}