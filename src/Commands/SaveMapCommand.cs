using RelEcs;
using Godot;

public class SaveMapCommand : ICommand
{
    public bool IsDone { get; set; }
    public bool IsRevertible { get; set; }
    public bool IsReverted { get; set; }
    
    public string MapName;
    
    public void Execute(World world)
    {
        world.SaveMap(MapName);
        IsDone = true;
    }

    public void Revert()
    {

    }
}