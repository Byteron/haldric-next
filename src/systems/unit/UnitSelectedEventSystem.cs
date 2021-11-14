using Godot;
using Bitron.Ecs;

public struct UnitSelectedEvent
{
    public EcsEntity Unit { get; set; }

    public UnitSelectedEvent(EcsEntity unit)
    {
        Unit = unit;
    }
}

public class UnitSelectedEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<UnitSelectedEvent>().End();

        foreach (var eventEntityId in query)
        {
            var map = world.GetResource<Map>();

            var unitEntity = world.Entity(eventEntityId).Get<UnitSelectedEvent>().Unit;

            var coords = unitEntity.Get<Coords>();
            var moves = unitEntity.Get<Attribute<Moves>>();

            var locEntity = map.Locations.Get(coords.Cube());

            if (world.TryGetResource<SelectedLocation>(out var selectedLocation))
            {
                selectedLocation.Entity = locEntity;
            }
            else
            {
                world.AddResource(new SelectedLocation(locEntity));
            }

            map.UpdateDistances(coords, unitEntity.Get<Side>().Value);

            world.Spawn().Add(new HighlightLocationEvent(coords, moves.Value));
        }
    }
}