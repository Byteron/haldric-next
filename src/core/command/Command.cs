using Godot;
using RelEcs;

public interface ICommandSystem : ISystem
{
    bool IsDone { get; set; }
    bool IsRevertable { get; set; }
    bool IsReverted { get; set; }
    void Revert();
}