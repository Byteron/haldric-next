using System.Collections.Generic;
using Bitron.Ecs;

public struct Commander
{
    private Stack<Command> History;
    private Queue<Command> Queue;

    public Command Peek()
    {
        return Queue.Peek();
    }

    public void Enqueue(Command command)
    {
        Queue.Enqueue(command);
    }

    public Command Dequeue()
    {
        var command = Queue.Dequeue();

        if (command.IsRevertable && !command.IsReverted)
        {
            History.Push(command);
        }

        return command;
    }

    public bool IsEmpty()
    {
        return Queue.Count == 0;
    }

    public void Undo()
    {
        if (History.Count == 0)
        {
            return;
        }

        var command = History.Pop();
        command.Revert();
        Queue.Enqueue(command);
    }

    public void ClearHistory()
    {
        History.Clear();
    }
}