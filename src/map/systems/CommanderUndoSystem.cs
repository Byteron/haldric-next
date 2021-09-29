using Godot;
using Bitron.Ecs;

public class CommanderUndoSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        ref var commander = ref world.GetResource<Commander>();

        if (Input.IsActionJustPressed("undo"))
        {
            commander.Undo();
        }
    }
}