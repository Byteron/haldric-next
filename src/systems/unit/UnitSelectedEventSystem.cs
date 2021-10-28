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
            var unitEntity = world.Entity(eventEntityId).Get<UnitSelectedEvent>().Unit;

            var moves = unitEntity.Get<Attribute<Moves>>();

            var coords = unitEntity.Get<Coords>();

            var map = world.GetResource<Map>();
            map.UpdateDistances(coords, unitEntity.Get<Team>().Value);
            
            world.Spawn().Add(new HighlightLocationEvent(coords, moves.Value));
        }
    }
}