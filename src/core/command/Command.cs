using Leopotam.Ecs;

public abstract class Command
{
    public bool IsRevertable { get; protected set; }

    public abstract void Revert();
}