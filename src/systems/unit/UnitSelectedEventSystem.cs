using Godot;
using Bitron.Ecs;

public struct UnitSelectedEvent
{
    public EcsEntity Unit;

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

            var ap = unitEntity.Get<Attribute<Moves>>();

            var coords = unitEntity.Get<Coords>();
            
            world.Spawn().Add(new HighlightLocationEvent(coords, ap.Value));
        }
    }
}