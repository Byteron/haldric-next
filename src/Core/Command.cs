using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RelEcs;

public interface ICommand
{
    bool IsDone { get; set; }
    bool IsRevertible { get; set; }
    bool IsReverted { get; set; }
    void Execute(World world);
    void Revert();
}

public class Commander
{
    readonly Stack<ICommand> _history = new();
    readonly Queue<ICommand> _queue = new();

    public void Enqueue(ICommand command)
    {
        _queue.Enqueue(command);
    }

    public ICommand Dequeue()
    {
        var command = _queue.Dequeue();

        if (command.IsRevertible && !command.IsReverted)
        {
            _history.Push(command);
        }

        return command;
    }

    public bool IsEmpty()
    {
        return _queue.Count == 0;
    }

    public void Undo()
    {
        if (_history.Count == 0) return;

        var command = _history.Pop();
        command.Revert();
        _queue.Enqueue(command);
    }

    public void ClearHistory()
    {
        _history.Clear();
    }
}

public static class CommanderSystems
{
    public static void Enqueue(this World world, ICommand command)
    {
        var commander = world.GetElement<Commander>();
        commander.Enqueue(command);;
    }
}