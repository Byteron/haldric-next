using RelEcs;

public class ExecuteCommandSystem : ISystem
{
     ICommandSystem _activeCommand;

    public void Run(Commands commands)
    {
        var commander = commands.GetElement<Commander>();

        if (_activeCommand is { IsDone: false }) return;

        while (!commander.IsEmpty())
        {
            _activeCommand = commander.Dequeue();

            if (!_activeCommand.IsRevertible) commander.ClearHistory();

            _activeCommand.Run(commands);

            if (!_activeCommand.IsDone) return;
        }

        _activeCommand = null;
        commands.GetElement<GameStateController>().PopState();
    }
}