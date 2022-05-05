using Godot;
using RelEcs;
using RelEcs.Godot;

public class SelectUnitSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        var hoveredLocEntity = hoveredLocation.Entity;

        if (!hoveredLocEntity.IsAlive)
        {
            return;
        }

        if (Input.IsActionJustPressed("select_unit") && hoveredLocEntity.Has<HasUnit>())
        {
            var scenario = commands.GetElement<Scenario>();
            var localPlayer = commands.GetElement<LocalPlayer>();

            var sideEntity = scenario.GetCurrentSideEntity();
            var playerId = sideEntity.Get<PlayerId>();
            var side = sideEntity.Get<Side>();

            if (playerId.Value != localPlayer.Id)
            {
                return;
            }

            var unitEntity = hoveredLocEntity.Get<HasUnit>().Entity;

            if (unitEntity.Get<Side>().Value != side.Value)
            {
                return;
            }

            if (commands.HasElement<SelectedLocation>())
            {
                commands.Send(new UnitDeselectedEvent());
            }

            commands.Send(new UnitSelectedEvent(unitEntity));
        }
    }
}