using Godot;
using Bitron.Ecs;

public class SelectUnitSystem : IEcsSystem
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

        if (Input.IsActionJustPressed("select_unit") && hoveredLocEntity.Has<HasUnit>())
        {
            var scenario = world.GetResource<Scenario>();
            var unitEntity = hoveredLocEntity.Get<HasUnit>().Entity;
            
            if (unitEntity.Get<Team>().Value != scenario.CurrentPlayer)
            {
                return;
            }

            if (world.TryGetResource<SelectedLocation>(out var selectedLocation))
            {
                selectedLocation.Entity = hoveredLocEntity;
                world.Spawn().Add(new UnitDeselectedEvent());
                world.Spawn().Add(new UnitSelectedEvent(unitEntity));
            }
            else
            {
                world.AddResource(new SelectedLocation(hoveredLocEntity));
                world.Spawn().Add(new UnitSelectedEvent(unitEntity));
            }
        }
    }
}