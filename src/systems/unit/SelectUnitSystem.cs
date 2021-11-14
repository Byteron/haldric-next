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
            var localPlayer = world.GetResource<LocalPlayer>();

            if (scenario.CurrentPlayer != localPlayer.Side)
            {
                return;
            }

            var unitEntity = hoveredLocEntity.Get<HasUnit>().Entity;

            if (unitEntity.Get<Side>().Value != scenario.CurrentPlayer)
            {
                return;
            }

            if (world.HasResource<SelectedLocation>())
            {
                world.Spawn().Add(new UnitDeselectedEvent());
            }
            
            world.Spawn().Add(new UnitSelectedEvent(unitEntity));
        }
    }
}