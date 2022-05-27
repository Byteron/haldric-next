using System.Collections.Generic;

public class Commander
{
     Stack<ICommandSystem> _history = new Stack<ICommandSystem>();
     Queue<ICommandSystem> _queue = new Queue<ICommandSystem>();

    public ICommandSystem Peek()
    {
        return _queue.Peek();
    }

    public void Enqueue(ICommandSystem command)
    {
        _queue.Enqueue(command);
    }

    public ICommandSystem Dequeue()
    {
        var command = _queue.Dequeue();

        if (command.IsRevertable && !command.IsReverted)
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
        if (_history.Count == 0)
        {
            return;
        }

        var command = _history.Pop();
        command.Revert();
        _queue.Enqueue(command);
    }

    public void ClearHistory()
    {
        _history.Clear();
    }
}