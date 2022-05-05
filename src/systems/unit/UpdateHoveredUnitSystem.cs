using Godot;
using RelEcs;
using RelEcs.Godot;

public class UpdateHoveredUnitSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        var hoveredLocEntity = hoveredLocation.Entity;

        if (!hoveredLocEntity.IsAlive)
        {
            return;
        }

        if (!hoveredLocEntity.Has<HasUnit>())
        {
            return;
        }

        var hoveredUnitEntity = hoveredLocEntity.Get<HasUnit>().Entity;

        commands.Send(new UnitHoveredEvent(hoveredUnitEntity));
    }
}