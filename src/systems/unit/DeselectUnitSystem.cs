using Godot;
using Bitron.Ecs;

public class DeselectUnitSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.TryGetResource<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        var locEntity = hoveredLocation.Entity;

        if (!locEntity.IsAlive())
        {
            return;
        }

        if (Input.IsActionJustPressed("deselect_unit"))
        {
            world.Spawn().Add(new UnitDeselectedEvent());
        }
    }
}