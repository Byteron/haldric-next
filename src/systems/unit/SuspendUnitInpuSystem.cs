using RelEcs;
using RelEcs.Godot;
using Godot;

public class SuspendUnitInputSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (Input.IsActionJustPressed("suspend_unit"))
        {
            if (!commands.TryGetElement<SelectedLocation>(out var selectedLocation))
            {
                return;
            }

            var scenario = commands.GetElement<Scenario>();
            var sideEntity = scenario.GetCurrentSideEntity();
            var side = sideEntity.Get<Side>();
            var playerId = sideEntity.Get<PlayerId>();

            if (side.Value == -1)
            {
                return;
            }

            var localPlayer = commands.GetElement<LocalPlayer>();

            if (playerId.Value != localPlayer.Id)
            {
                return;
            }

            var unitEntity = selectedLocation.Entity.Get<HasUnit>().Entity;

            var unitSide = unitEntity.Get<Side>();

            if (unitSide.Value == side.Value)
            {
                if (unitEntity.Has<Suspended>())
                {
                    unitEntity.Remove<Suspended>();
                }
                else
                {
                    unitEntity.Add(new Suspended());
                    commands.Send(new UnitDeselectedEvent());
                }
            }
        }
    }
}