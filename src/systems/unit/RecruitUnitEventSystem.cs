using Godot;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public class RecruitUnitEvent
{
    public int Side;
    public UnitType UnitType;
    public Entity LocEntity;

    public RecruitUnitEvent(int side, UnitType unitType, Entity locEntity)
    {
        Side = side;
        UnitType = unitType;
        LocEntity = locEntity;
    }
}

public class RecruitUnitEventSystem : ISystem
{
    Node3D _parent;

    public RecruitUnitEventSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(Commands commands)
    {
        if (Data.Instance.Units.Count == 0)
        {
            return;
        }

        commands.Receive((RecruitUnitEvent recruitEvent) =>
        {
            var scenario = commands.GetElement<Scenario>();
            var sideEntity = scenario.GetCurrentSideEntity();

            var gold = sideEntity.Get<Gold>();

            var locEntity = recruitEvent.LocEntity;

            if (locEntity.Has<HasUnit>())
            {
                GD.PrintErr("Location already has a unit! Not recruiting: " + recruitEvent.UnitType.Name);
                return;
            }
            
            var freeCoords = locEntity.Get<Coords>();

            var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
            var elevationOffset = terrainEntity.Get<ElevationOffset>();

            UnitType unitType = recruitEvent.UnitType;
            _parent.AddChild(unitType);
            UnitView unitView = unitType.UnitView;
            unitType.RemoveChild(unitView);
            _parent.AddChild(unitView);

            var unitEntity = UnitFactory.CreateFromUnitType(commands, unitType, unitView);

            gold.Value -= unitType.Cost;

            unitType.QueueFree();

            var position = freeCoords.World();
            position.y = locEntity.Get<Elevation>().Height + elevationOffset.Value;

            unitView.Position = position;

            unitEntity.Add(new Side { Value = recruitEvent.Side });
            unitEntity.Add(freeCoords);

            unitEntity.Get<Attribute<Moves>>().Empty();
            unitEntity.Get<Attribute<Actions>>().Empty();

            var canvas = commands.GetElement<Canvas>();
            var canvasLayer = canvas.GetCanvasLayer(0);

            var unitPlate = Scenes.Instantiate<UnitPlate>();
            
            canvasLayer.AddChild(unitPlate);

            unitEntity.Add(unitPlate);

            recruitEvent.LocEntity.Add(new HasUnit { Entity = unitEntity });
        });
    }
}
