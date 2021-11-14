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
            var player = scenario.GetCurrentPlayerEntity();
            ref var playerSide = ref player.Get<Side>();

            if (playerSide.Value == -1)
            {
                return;
            }

            if (playerSide.Value != localPlayer.Side)
            {
                return;
            }

            foreach (var unitId in unitQuery)
            {
                var unitEntity = world.Entity(unitId);

                ref var side = ref unitEntity.Get<Side>();
                ref var moves = ref unitEntity.Get<Attribute<Moves>>();

                if (side.Value == playerSide.Value && !moves.IsEmpty())
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