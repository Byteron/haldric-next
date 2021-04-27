using Godot;
using Leopotam.Ecs;

public struct UpdateMapEvent {}

public class UpdateMapEventSystem : IEcsRunSystem
{
    EcsFilter<UpdateMapEvent> _events;
    EcsFilter<ViewHandle<MapView>, Locations> _maps;

    public void Run()
    {
        foreach (var i in _events)
        {
            GD.Print("UpdateMapEvent Sent!");
            foreach (var j in _maps)
            {
                var mapEntity = _maps.GetEntity(j);

                ref var locations = ref mapEntity.Get<Locations>();
                ref var mapView = ref mapEntity.Get<ViewHandle<MapView>>();

                mapView.Node.Build(locations);
            }

            _events.GetEntity(i).Destroy();
        }
    }
}