using RelEcs;
using Godot;

public partial class MoveUnitCommand : RefCounted, ICommandSystem
{
    public bool IsDone { get; set; }
    public bool IsRevertible { get; set; }
    public bool IsReverted { get; set; }
    public World World { get; set; }

    public void Revert()
    {

    }

    public void Run()
    {

    }
}