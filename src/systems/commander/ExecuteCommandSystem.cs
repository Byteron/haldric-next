using Godot;
using Bitron.Ecs;

public class ExecuteCommandSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var commander = world.GetResource<Commander>();

        while (!commander.IsEmpty())
        {
            var command = commander.Dequeue();
            command.Execute();
        }

        world.GetResource<GameStateController>().PopState();
    }
}