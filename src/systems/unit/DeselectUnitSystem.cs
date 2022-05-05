using Godot;
using RelEcs;
using RelEcs.Godot;

public class DeselectUnitSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        var locEntity = hoveredLocation.Entity;

        if (!locEntity.IsAlive)
        {
            return;
        }

        if (Input.IsActionJustPressed("deselect_unit"))
        {
            commands.Send(new UnitDeselectedEvent());
        }
    }
}