using Godot;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public class SpawnUnitTrigger
{
    public int Side { get; set; }
    public string Id { get; set; }
    public Coords Coords { get; set; }
    public bool IsLeader { get; set; }
    public bool IsHero { get; set; }

    public SpawnUnitTrigger(int side, string id, Coords coords, bool isLeader = false, bool isHero = false)
    {
        Side = side;
        Id = id;
        Coords = coords;
        IsLeader = isLeader;
        IsHero = isHero;
    }
}

public class SpawnUnitTriggerSystem : ISystem
{
    readonly Node3D _parent;

    public SpawnUnitTriggerSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(Commands commands)
    {
        if (Data.Instance.Units.Count == 0) return;

        commands.Receive((SpawnUnitTrigger spawnEvent) =>
        {
            var map = commands.GetElement<Map>();

            var locations = map.Locations;

            var locEntity = locations.Get(spawnEvent.Coords.Cube());
            var elevation = locEntity.Get<Elevation>();

            var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
            var elevationOffset = terrainEntity.Get<ElevationOffset>();

            var unitType = Data.Instance.Units[spawnEvent.Id].Instantiate<UnitType>();
            _parent.AddChild(unitType);
            var unitView = unitType.UnitView;
            unitType.RemoveChild(unitView);
            _parent.AddChild(unitView);

            var unitEntity = UnitFactory.CreateFromUnitType(commands, unitType, unitView);

            unitType.QueueFree();

            var position = spawnEvent.Coords.World();
            position.y = elevation.Height + elevationOffset.Value;

            unitView.Position = position;

            unitEntity.Add(new Side { Value = spawnEvent.Side });
            unitEntity.Add(spawnEvent.Coords.Clone());

            if (spawnEvent.IsLeader)
            {
                unitEntity.Add(new IsLeader());
            }

            if (spawnEvent.IsHero)
            {
                unitEntity.Add(new IsHero());
            }

            var canvas = commands.GetElement<Canvas>();
            var canvasLayer = canvas.GetCanvasLayer(0);

            var unitPlate = Scenes.Instantiate<UnitPlate>();

            canvasLayer.AddChild(unitPlate);

            unitEntity.Add(unitPlate);

            locEntity.Add(new HasUnit { Entity = unitEntity });
        });
    }
}
