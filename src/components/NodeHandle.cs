using Godot;
using Bitron.Ecs;

public struct NodeHandle<T> : IEcsAutoReset<NodeHandle<T>> where T : Node
{
    public T Node { get; set; }

    public NodeHandle(T node)
    {
        Node = node;
    }

    public void AutoReset(ref NodeHandle<T> c)
    {
        c.Node = null;
    }
}