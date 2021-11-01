using System.Collections.Generic;
using Godot;
using Bitron.Ecs;

public struct UpdateMapEvent
{
    public List<Vector3i> Chunks { get; set; }

    public UpdateMapEvent(List<Vector3i> chunks = null)
    {
        Chunks = chunks;
    }
}

public class UpdateMapEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<UpdateMapEvent>().End();

        foreach (var eventEntityId in eventQuery)
        {
            var updateEvent = world.Entity(eventEntityId).Get<UpdateMapEvent>();

            SendUpdateTerrainMeshEvent(world, updateEvent);
            SendUpdateTerrainFeaturePopulatorEvent(world, updateEvent);
        }
    }

    private void SendUpdateTerrainMeshEvent(EcsWorld world, UpdateMapEvent e)
    {
        world.Spawn().Add(new UpdateTerrainMeshEvent(e.Chunks));
    }

    private void SendUpdateTerrainFeaturePopulatorEvent(EcsWorld world, UpdateMapEvent e)
    {
        world.Spawn().Add(new UpdateTerrainFeaturePopulatorEvent(e.Chunks));
    }
}