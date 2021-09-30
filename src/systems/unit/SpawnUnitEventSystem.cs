using Godot;
using Bitron.Ecs;
using System.Linq;

public struct SpawnUnitEvent
{
    public Coords Coords;
    public string Id;

    public SpawnUnitEvent(string id, Coords coords)
    {
        Id = id;
        Coords = coords;
    }
}

public class SpawnUnitEventSystem : IEcsSystem
{
    Node3D _parent;

    public SpawnUnitEventSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        if (Data.Instance.UnitDicts.Count == 0)
        {
            return;
        }

        var eventQuery = world.Query<SpawnUnitEvent>().End();
        var mapQuery = world.Query<Locations>().Inc<Map>().End();

        foreach (var eventEntityId in eventQuery)
        {
            foreach (var mapEntityId in mapQuery)
            {
                ref var spawnEvent = ref eventQuery.Get<SpawnUnitEvent>(eventEntityId);

                ref var locations = ref mapQuery.Get<Locations>(mapEntityId);

                var locEntity = locations.Get(spawnEvent.Coords.Cube);
                ref var elevation = ref locEntity.Get<Elevation>();

                string key = Data.Instance.UnitDicts.Keys.ToArray<string>()[GD.Randi() % Data.Instance.UnitDicts.Count];
                var unitEntity = UnitFactory.CreateFromDict(Data.Instance.UnitDicts[key]);

                var unitView = unitEntity.Get<AssetHandle<PackedScene>>().Asset.Instantiate<UnitView>();

                unitEntity.Remove<AssetHandle<PackedScene>>();

                _parent.AddChild(unitView);

                var position = spawnEvent.Coords.World;
                position.y = elevation.Height;

                unitView.Position = position;

                unitEntity.Add(spawnEvent.Coords);
                unitEntity.Add(new NodeHandle<UnitView>(unitView));

                locEntity.Add(new HasUnit(unitEntity));
            }
        }
    }
}
