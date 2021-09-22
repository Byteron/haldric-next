using System.Collections.Generic;
using Godot;
using Leopotam.Ecs;

public struct UpdateMapEvent
{
    public List<Vector3i> Chunks;

    public UpdateMapEvent(List<Vector3i> chunks = null)
    {
        Chunks = chunks;
    }
}

public class UpdateMapEventSystem : IEcsRunSystem
{
    EcsWorld _world;
    EcsFilter<UpdateMapEvent> _events;

    public void Run()
    {
        foreach (var i in _events)
        {
            var eventEntity = _events.GetEntity(i);
            var updateEvent = eventEntity.Get<UpdateMapEvent>();

            SendUpdateTerrainMeshEvent(updateEvent);
            SendUpdateTerrainFeaturePopulatorEvent(updateEvent);
        }
    }

    private void SendUpdateTerrainMeshEvent(UpdateMapEvent e)
    {
        var updateTerrainMeshEvent = _world.NewEntity();
        updateTerrainMeshEvent.Replace(new UpdateTerrainMeshEvent(e.Chunks));
    }

    private void SendUpdateTerrainFeaturePopulatorEvent(UpdateMapEvent e)
    {
        var updateTerrainFeaturePopulatorEvent = _world.NewEntity();
        updateTerrainFeaturePopulatorEvent.Replace(new UpdateTerrainFeaturePopulatorEvent(e.Chunks));
    }
}