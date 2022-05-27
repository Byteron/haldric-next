using Godot;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public class RecruitUnitTrigger
{
    public int Side;
    public UnitType UnitType;
    public Entity LocEntity;

    public RecruitUnitTrigger(int side, UnitType unitType, Entity locEntity)
    {
        Side = side;
        UnitType = unitType;
        LocEntity = locEntity;
    }
}

public class RecruitUnitTriggerSystem : ISystem
{
    Node3D _parent;

    public RecruitUnitTriggerSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(Commands commands)
    {
        if (Data.Instance.Units.Count == 0) return;

        commands.Receive((RecruitUnitTrigger t) =>
        {
            var scenario = commands.GetElement<Scenario>();
            var sideEntity = scenario.GetCurrentSideEntity();

            var gold = sideEntity.Get<Gold>();

            var locEntity = t.LocEntity;

            if (locEntity.Has<HasUnit>())
            {
                GD.PrintErr("Location already has a unit! Not recruiting: " + t.UnitType.Name);
                return;
            }
            
            var freeCoords = locEntity.Get<Coords>();

            var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
            var elevationOffset = terrainEntity.Get<ElevationOffset>();

            var unitType = t.UnitType;
            _parent.AddChild(unitType);
            var unitView = unitType.UnitView;
            unitType.RemoveChild(unitView);
            _parent.AddChild(unitView);

            var unitEntity = UnitFactory.CreateFromUnitType(commands, unitType, unitView);

            gold.Value -= unitType.Cost;

            unitType.QueueFree();

            var position = freeCoords.World();
            position.y = locEntity.Get<Elevation>().Height + elevationOffset.Value;

            unitView.Position = position;

            unitEntity.Add(new Side { Value = t.Side });
            unitEntity.Add(freeCoords.Clone());

            unitEntity.Get<Attribute<Moves>>().Empty();
            unitEntity.Get<Attribute<Actions>>().Empty();

            var canvas = commands.GetElement<Canvas>();
            var canvasLayer = canvas.GetCanvasLayer(0);

            var unitPlate = Scenes.Instantiate<UnitPlate>();
            
            canvasLayer.AddChild(unitPlate);

            unitEntity.Add(unitPlate);

            t.LocEntity.Add(new HasUnit { Entity = unitEntity });
        });
    }
}
