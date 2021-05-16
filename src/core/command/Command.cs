using Leopotam.Ecs;

public abstract class Command
{
    public bool IsRevertable { get; protected set; } = false;
    public bool IsReverted { get; protected set; } = false;
    public abstract void Revert();
}