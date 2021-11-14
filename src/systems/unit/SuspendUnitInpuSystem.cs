using Bitron.Ecs;
using Godot;

public class SuspendUnitInputSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (Input.IsActionJustPressed("suspend_unit"))
        {
            if (!world.TryGetResource<SelectedLocation>(out var selectedLocation))
            {
                return;
            }

            var scenario = world.GetResource<Scenario>();
            var player = scenario.GetCurrentPlayerEntity();
            ref var playerSide = ref player.Get<Side>();

            if (playerSide.Value == -1)
            {
                return;
            }

            var localPlayer = world.GetResource<LocalPlayer>();

            if (playerSide.Value != localPlayer.Side)
            {
                return;
            }

            var unitEntity = selectedLocation.Entity.Get<HasUnit>().Entity;

            ref var unitSide = ref unitEntity.Get<Side>();

            if (unitSide.Value == playerSide.Value)
            {
                if (unitEntity.Has<Suspended>())
                {
                    unitEntity.Remove<Suspended>();
                }
                else
                {
                    unitEntity.Add(new Suspended());
                    world.Spawn().Add(new UnitDeselectedEvent());
                }
            }
        }
    }
}