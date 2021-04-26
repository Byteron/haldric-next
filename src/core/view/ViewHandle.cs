using Godot;

public struct ViewHandle<T> where T: Node
{
    public T Node;

    public ViewHandle(T node)
    {
        Node = node;
    }
}