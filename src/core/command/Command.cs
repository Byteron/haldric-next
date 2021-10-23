using Godot;
using Bitron.Ecs;

public abstract partial class Command : RefCounted
{
    public bool IsDone { get; protected set; } = false;
    public bool IsRevertable { get; protected set; } = false;
    public bool IsReverted { get; protected set; } = false;
    public abstract void Revert();
    public abstract void Execute();
}