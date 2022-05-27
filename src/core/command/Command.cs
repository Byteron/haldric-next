using RelEcs;

public interface ICommandSystem : ISystem
{
    bool IsDone { get; set; }
    bool IsRevertible { get; set; }
    bool IsReverted { get; set; }
    void Revert();
}