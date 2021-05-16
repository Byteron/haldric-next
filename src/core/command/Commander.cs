using System.Collections.Generic;
using Leopotam.Ecs;

public struct Commander : IEcsAutoReset<Commander>
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

        if (command.IsRevertable)
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

    public void AutoReset(ref Commander c)
    {
        c.History = new Stack<Command>();
        c.Queue = new Queue<Command>();
    }
}