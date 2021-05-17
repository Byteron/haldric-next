using Godot;
using Leopotam.Ecs;
using System.Linq;

public struct CreateUnitEvent
{
    public Coords Coords;
    public string Id;

    public CreateUnitEvent(string id, Coords coords)
    {
        Id = id;
        Coords = coords;
    }
}

public class CreateUnitEventSystem : IEcsRunSystem
{
    Node3D _parent;

    EcsWorld _world;
    EcsFilter<CreateUnitEvent> _events;
    EcsFilter<Locations, Map> _maps;

    public CreateUnitEventSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run()
    {
        if (_maps.IsEmpty())
        {
            return;
        }

        foreach (var i in _events)
        {
            var eventEntity = _events.GetEntity(i);
            ref var createEvent = ref eventEntity.Get<CreateUnitEvent>();

            var mapEntity = _maps.GetEntity(0);
            ref var locations = ref mapEntity.Get<Locations>();

            var locEntity = locations.Get(createEvent.Coords.Cube);
            ref var elevation = ref locEntity.Get<Elevation>();

            string key = Data.Instance.Units.Keys.ToArray<string>()[GD.Randi() % Data.Instance.Units.Count];
            var unitEntity = Data.Instance.Units[key].Copy();
            
            var unitView = unitEntity.Get<AssetHandle<PackedScene>>().Asset.Instance<UnitView>();
            
            unitEntity.Del<AssetHandle<PackedScene>>();

            _parent.AddChild(unitView);

            var translation = createEvent.Coords.World;
            translation.y = elevation.Height;

            unitView.Translation = translation;

            unitEntity.Replace(createEvent.Coords);
            unitEntity.Replace(new NodeHandle<UnitView>(unitView));

            locEntity.Replace(new HasUnit(unitEntity));

            eventEntity.Destroy();
        }
    }
}
