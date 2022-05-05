using RelEcs;
using RelEcs.Godot;
using Godot;

public class NextUnitInputSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (Input.IsActionJustPressed("next_unit"))
        {
            var unitQuery = commands.Query().Has<Side, Attribute<Moves>>().Not<Suspended>();

            var map = commands.GetElement<Map>();
            var scenario = commands.GetElement<Scenario>();
            var localPlayer = commands.GetElement<LocalPlayer>();

            var sideEntity = scenario.GetCurrentSideEntity();
            ref var side = ref sideEntity.Get<Side>();
            ref var playerId = ref sideEntity.Get<PlayerId>();

            if (playerId.Value != localPlayer.Id)
            {
                return;
            }

            foreach (var unitEntity in unitQuery)
            {
                ref var unitSide = ref unitEntity.Get<Side>();
                ref var moves = ref unitEntity.Get<Attribute<Moves>>();

                if (unitSide.Value == side.Value && !moves.IsEmpty())
                {
                    ref var coords = ref unitEntity.Get<Coords>();

                    var locEntity = map.Locations.Get(coords.Cube());

                    if (commands.TryGetElement<SelectedLocation>(out var selectedLocation))
                    {
                        commands.Send(new UnitDeselectedEvent());
                    }

                    commands.Send(new FocusCameraEvent(coords));
                    commands.Send(new UnitSelectedEvent(unitEntity));
                    return;
                }
            }

        }
    }
}