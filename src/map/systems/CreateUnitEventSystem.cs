using Godot;
using Bitron.Ecs;
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

public class CreateUnitEventSystem : IEcsSystem
{
    Node3D _parent;

    public CreateUnitEventSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        if (Data.Instance.Units.Count == 0)
        {
            return;
        }

        var eventQuery = world.Query<CreateUnitEvent>().End();
        var mapQuery = world.Query<Locations>().Inc<Map>().End();

        foreach (var eventEntityId in eventQuery)
        {
            foreach (var mapEntityId in mapQuery)
            {
                ref var createEvent = ref eventQuery.Get<CreateUnitEvent>(eventEntityId);

                ref var locations = ref mapQuery.Get<Locations>(mapEntityId);

                var locEntity = locations.Get(createEvent.Coords.Cube);
                ref var elevation = ref locEntity.Get<Elevation>();

                string key = Data.Instance.Units.Keys.ToArray<string>()[GD.Randi() % Data.Instance.Units.Count];
                var unitEntity = UnitFactory.CreateFromDict(Data.Instance.Units[key]);

                var unitView = unitEntity.Get<AssetHandle<PackedScene>>().Asset.Instantiate<UnitView>();

                unitEntity.Remove<AssetHandle<PackedScene>>();

                _parent.AddChild(unitView);

                var position = createEvent.Coords.World;
                position.y = elevation.Height;

                unitView.Position = position;

                unitEntity.Add(createEvent.Coords);
                unitEntity.Add(new NodeHandle<UnitView>(unitView));

                locEntity.Add(new HasUnit(unitEntity));
            }
        }
    }
}
