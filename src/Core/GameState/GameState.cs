using Godot;
using RelEcs;

public partial class GameState : Node
{
    public readonly SystemGroup Enter = new();
    public readonly SystemGroup Update = new();
    public readonly SystemGroup Continue = new();
    public readonly SystemGroup Pause = new();
    public readonly SystemGroup Exit = new();

    public virtual void Init() { }
}