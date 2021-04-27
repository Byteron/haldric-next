using Godot;
using Leopotam.Ecs;

public struct ViewHandle<T> : IEcsAutoReset<ViewHandle<T>> where T : Node
{
    public T Node;

    public ViewHandle(T node)
    {
        Node = node;
    }

    public void AutoReset(ref ViewHandle<T> c)
    {
        c.Node = null;
    }
}