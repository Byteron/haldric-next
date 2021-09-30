using Godot;
using Bitron.Ecs;

public class CommanderUndoSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var commander = world.GetResource<Commander>();

        if (Input.IsActionJustPressed("undo"))
        {
            commander.Undo();
        }
    }
}