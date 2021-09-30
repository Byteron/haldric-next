using Godot;
using Bitron.Ecs;

public struct NodeHandle<T> : IEcsAutoReset<NodeHandle<T>> where T : Node
{
    public T Node;

    public NodeHandle(T node)
    {
        Node = node;
    }

    public void AutoReset(ref NodeHandle<T> c)
    {
        if (c.Node != null)
        {
            c.Node.QueueFree();
        }
        
        c.Node = null;
    }
}