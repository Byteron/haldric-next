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
            var sideEntity = scenario.GetCurrentSideEntity();
            ref var side = ref sideEntity.Get<Side>();
            ref var playerId = ref sideEntity.Get<PlayerId>();

            if (side.Value == -1)
            {
                return;
            }

            var localPlayer = world.GetResource<LocalPlayer>();

            if (playerId.Value != localPlayer.Id)
            {
                return;
            }

            var unitEntity = selectedLocation.Entity.Get<HasUnit>().Entity;

            ref var unitSide = ref unitEntity.Get<Side>();

            if (unitSide.Value == side.Value)
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