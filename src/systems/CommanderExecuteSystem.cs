using Godot;
using Bitron.Ecs;

public class CommanderExecuteSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var commander = world.GetResource<Commander>();

        while (!commander.IsEmpty())
        {
            var command = commander.Dequeue();
            command.Execute();
        }
    }
}