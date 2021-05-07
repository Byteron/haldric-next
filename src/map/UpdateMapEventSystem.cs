using Godot;
using Leopotam.Ecs;

public struct UpdateMapEvent {}

public class UpdateMapEventSystem : IEcsRunSystem
{
    EcsWorld _world;
    EcsFilter<UpdateMapEvent> _events;

    public void Run()
    {
        foreach (var i in _events)
        {
            SendUpdateTerrainMeshEvent();
            SendUpdateTerrainFeaturePopulatorEvent();

            _events.GetEntity(i).Destroy();
        }
    }

    private void SendUpdateTerrainMeshEvent()
    {
        var updateTerrainMeshEvent = _world.NewEntity();
        updateTerrainMeshEvent.Get<UpdateTerrainMeshEvent>();
    }

    private void SendUpdateTerrainFeaturePopulatorEvent()
    {
        var updateTerrainFeaturePopulatorEvent = _world.NewEntity();
        updateTerrainFeaturePopulatorEvent.Get<UpdateTerrainFeaturePopulatorEvent>();
    }
}