using System.Collections.Generic;

public class Commander
{
     Stack<ICommandSystem> History = new Stack<ICommandSystem>();
     Queue<ICommandSystem> Queue = new Queue<ICommandSystem>();

    public ICommandSystem Peek()
    {
        return Queue.Peek();
    }

    public void Enqueue(ICommandSystem command)
    {
        Queue.Enqueue(command);
    }

    public ICommandSystem Dequeue()
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