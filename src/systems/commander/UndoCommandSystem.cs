using Godot;
using RelEcs;

public class UndoCommandSystem : ISystem
{
    public void Run(Commands commands)
    {
        var commander = commands.GetElement<Commander>();
        var gameStateController = commands.GetElement<GameStateController>();
        
        if (!Input.IsActionJustPressed("undo")) return;
        
        commander.Undo();
        gameStateController.PushState(new CommanderState());
    }
}