using Godot;
using Bitron.Ecs;
using Haldric.Wdk;

public struct SpawnUnitEvent
{
    public int Side { get; set; }
    public string Id { get; set; }
    public Coords Coords { get; set; }
    public bool IsLeader { get; set; }
    public bool IsHero { get; set; }

    public SpawnUnitEvent(int side, string id, Coords coords, bool isLeader = false, bool isHero = false)
    {
        Side = side;
        Id = id;
        Coords = coords;
        IsLeader = isLeader;
        IsHero = isHero;
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

        world.ForEach((ref SpawnUnitEvent spawnEvent) =>
        {
            var map = world.GetResource<Map>();

            var locations = map.Locations;

            var locEntity = locations.Get(spawnEvent.Coords.Cube());
            ref var elevation = ref locEntity.Get<Elevation>();

            var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
            ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();

            UnitType unitType = Data.Instance.Units[spawnEvent.Id].Instantiate<UnitType>();
            _parent.AddChild(unitType);
            UnitView unitView = unitType.UnitView;
            unitType.RemoveChild(unitView);
            _parent.AddChild(unitView);

            var unitEntity = UnitFactory.CreateFromUnitType(world, unitType, unitView);

            unitType.QueueFree();

            var position = spawnEvent.Coords.World();
            position.y = elevation.Height + elevationOffset.Value;

            unitView.Position = position;

            unitEntity.Add(new Side(spawnEvent.Side));
            unitEntity.Add(spawnEvent.Coords);

            if (spawnEvent.IsLeader)
            {
                unitEntity.Add(new IsLeader());
            }

            if (spawnEvent.IsHero)
            {
                unitEntity.Add(new IsHero());
            }

            var canvas = world.GetResource<Canvas>();
            var canvasLayer = canvas.GetCanvasLayer(0);

            var unitPlate = UnitPlate.Instantiate();

            canvasLayer.AddChild(unitPlate);

            unitEntity.Add(new NodeHandle<UnitPlate>(unitPlate));

            locEntity.Add(new HasUnit(unitEntity));
        });
    }
}
