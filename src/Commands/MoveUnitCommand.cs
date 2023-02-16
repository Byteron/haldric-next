using RelEcs;
using Godot;

public class MoveUnitCommand : ICommand
{
    public bool IsDone { get; set; }
    public bool IsRevertible { get; set; }
    public bool IsReverted { get; set; }

    public void Execute(World world)
    {
        
    }

    public void Revert()
    {

    }
}