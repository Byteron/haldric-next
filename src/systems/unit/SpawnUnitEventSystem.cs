using Godot;
using Bitron.Ecs;
using System.Linq;

public struct SpawnUnitEvent
{
    public int Team;
    public string Id;
    public Coords Coords;

    public SpawnUnitEvent(int team, string id, Coords coords)
    {
        Team = team;
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
        if (Data.Instance.Units.Count == 0)
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

                UnitType unitType = Data.Instance.Units[spawnEvent.Id].Instantiate<UnitType>();
                _parent.AddChild(unitType);
                UnitView unitView = unitType.UnitView;
                unitType.RemoveChild(unitView);
                _parent.AddChild(unitView);

                var unitEntity = UnitFactory.CreateFromUnitType(world, unitType, unitView);

                unitType.QueueFree();

                var position = spawnEvent.Coords.World;
                position.y = elevation.Height;

                unitView.Position = position;

                unitEntity.Add(new Team(spawnEvent.Team));
                unitEntity.Add(spawnEvent.Coords);

                locEntity.Add(new HasUnit(unitEntity));
            }
        }
    }
}
