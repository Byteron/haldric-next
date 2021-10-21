using Godot;
using Bitron.Ecs;

public class UndoCommandSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var commander = world.GetResource<Commander>();
        var gameStateController = world.GetResource<GameStateController>();
        if (Input.IsActionJustPressed("undo"))
        {
            commander.Undo();
            gameStateController.PushState(new CommanderState(world));
        }
    }
}