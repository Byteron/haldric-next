using Bitron.Ecs;

public class ExecuteCommandSystem : IEcsSystem
{
    private Command _activeCommand;

    public void Run(EcsWorld world)
    {
        var commander = world.GetResource<Commander>();

        if (_activeCommand != null && !_activeCommand.IsDone)
        {
            return;
        }

        while (!commander.IsEmpty())
        {
            _activeCommand = commander.Dequeue();

            if (!_activeCommand.IsRevertable)
            {
                commander.ClearHistory();
            }

            _activeCommand.Execute();

            if (!_activeCommand.IsDone)
            {
                return;
            }

        }

        _activeCommand = null;
        world.GetResource<GameStateController>().PopState();
    }
}