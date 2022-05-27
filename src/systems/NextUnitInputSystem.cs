using RelEcs;
using RelEcs.Godot;
using Godot;

public class NextUnitInputSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!Input.IsActionJustPressed("next_unit")) return;
        
        var unitQuery = commands.Query<Entity>().Has<Side>().Has<Attribute<Moves>>().Not<Suspended>();

        var scenario = commands.GetElement<Scenario>();
        var localPlayer = commands.GetElement<LocalPlayer>();

        var sideEntity = scenario.GetCurrentSideEntity();
        var side = sideEntity.Get<Side>();
        var playerId = sideEntity.Get<PlayerId>();

        if (playerId.Value != localPlayer.Id) return;

        foreach (var unitEntity in unitQuery)
        {
            var unitSide = unitEntity.Get<Side>();
            var moves = unitEntity.Get<Attribute<Moves>>();

            if (unitSide.Value != side.Value || moves.IsEmpty()) continue;
            
            var coords = unitEntity.Get<Coords>();

            if (commands.HasElement<SelectedLocation>())
            {
                commands.Send(new UnitDeselectedEvent());
            }

            commands.Send(new FocusCameraEvent(coords));
            commands.Send(new UnitSelectedEvent(unitEntity));
        }
    }
}