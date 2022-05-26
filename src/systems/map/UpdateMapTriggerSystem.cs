using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public class UpdateMapTrigger
{
    public List<Vector3i> Chunks { get; }
    
    public UpdateMapTrigger(List<Vector3i> chunks = null)
    {
        Chunks = chunks;
    }
}

public class UpdateMapTriggerSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((UpdateMapTrigger e) =>
        {
            commands.Send(new UpdateTerrainMeshEvent(e.Chunks));
            commands.Send(new UpdateTerrainFeaturePopulatorEvent(e.Chunks));
        });
    }
}