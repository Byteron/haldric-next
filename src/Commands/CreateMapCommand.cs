using RelEcs;
using Godot;

public class CreatMapCommand : ICommand
{
    public bool IsDone { get; set; }
    public bool IsRevertible { get; set; }
    public bool IsReverted { get; set; }

    public int Width;
    public int Height;
    
    public void Execute(World world)
    {
        world.DespawnMap();
        world.SpawnMap(Width, Height);
        
        IsDone = true;
    }

    public void Revert()
    {
    }
}