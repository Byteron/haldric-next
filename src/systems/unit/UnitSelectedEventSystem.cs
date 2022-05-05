using Godot;
using RelEcs;
using RelEcs.Godot;

public struct UnitSelectedEvent
{
    public Entity Unit { get; set; }

    public UnitSelectedEvent(Entity unit)
    {
        Unit = unit;
    }
}

public class UnitSelectedEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((UnitSelectedEvent e) => {
            var map = commands.GetElement<Map>();

            var unitEntity = e.Unit;

            var coords = unitEntity.Get<Coords>();
            var moves = unitEntity.Get<Attribute<Moves>>();

            var locEntity = map.Locations.Get(coords.Cube());

            if (commands.TryGetElement<SelectedLocation>(out var selectedLocation))
            {
                selectedLocation.Entity = locEntity;
            }
            else
            {
                commands.AddElement(new SelectedLocation(locEntity));
            }

            map.UpdateDistances(coords, unitEntity.Get<Side>().Value);

            commands.Send(new HighlightLocationEvent(coords, moves.Value));
        });
    }
}