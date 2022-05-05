using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public struct UpdateMapEvent
{
    public List<Vector3i> Chunks { get; set; }

    public UpdateMapEvent(List<Vector3i> chunks = null)
    {
        Chunks = chunks;
    }
}

public class UpdateMapEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((UpdateMapEvent e) =>
        {
            commands.Send(new UpdateTerrainMeshEvent(e.Chunks));
            commands.Send(new UpdateTerrainFeaturePopulatorEvent(e.Chunks));
        });
    }
}