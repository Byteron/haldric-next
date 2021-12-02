using Bitron.Ecs;
using Godot;

public class NextUnitInputSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (Input.IsActionJustPressed("next_unit"))
        {
            var unitQuery = world.Query<Attribute<Moves>>().Inc<Side>().Exc<Suspended>().End();
            var map = world.GetResource<Map>();
            var scenario = world.GetResource<Scenario>();
            var localPlayer = world.GetResource<LocalPlayer>();

            var sideEntity = scenario.GetCurrentSideEntity();
            ref var side = ref sideEntity.Get<Side>();
            ref var playerId = ref sideEntity.Get<PlayerId>();

            if (playerId.Value != localPlayer.Id)
            {
                return;
            }

            foreach (var unitId in unitQuery)
            {
                var unitEntity = world.Entity(unitId);

                ref var unitSide = ref unitEntity.Get<Side>();
                ref var moves = ref unitEntity.Get<Attribute<Moves>>();

                if (unitSide.Value == side.Value && !moves.IsEmpty())
                {
                    ref var coords = ref unitEntity.Get<Coords>();

                    var locEntity = map.Locations.Get(coords.Cube());

                    if (world.TryGetResource<SelectedLocation>(out var selectedLocation))
                    {
                        world.Spawn().Add(new UnitDeselectedEvent());
                    }

                    world.Spawn().Add(new FocusCameraEvent(coords));
                    world.Spawn().Add(new UnitSelectedEvent(unitEntity));
                    return;
                }
            }

        }
    }
}