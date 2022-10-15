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

        world.DespawnMap();
        var mapData = scenarioData.Maps[MapName];
        world.SpawnMap(mapData);

        IsDone = true;
    }

    public void Revert()
    {

    }
}