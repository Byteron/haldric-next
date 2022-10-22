using RelEcs;
using Godot;

public class LoadMapCommand : ICommand
{
    public bool IsDone { get; set; }
    public bool IsRevertible { get; set; }
    public bool IsReverted { get; set; }

    public string MapName;
    
    public void Execute(World world)
    {
        var scenarioData = world.GetElement<ScenarioData>();

        DespawnMap(world);
        var mapData = scenarioData.Maps[MapName];
        SpawnMap(world, mapData);

        IsDone = true;
    }

    public void Revert()
    {

    }
}