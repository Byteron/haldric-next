using System.Collections.Generic;
using RelEcs;
using Godot;

public class CombatCommand : ICommand
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