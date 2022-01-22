using Godot;
using Bitron.Ecs;
using Haldric.Wdk;

public struct RecruitUnitEvent
{
    public int Side;
    public UnitType UnitType;
    public EcsEntity LocEntity;

    public RecruitUnitEvent(int side, UnitType unitType, EcsEntity locEntity)
    {
        Side = side;
        UnitType = unitType;
        LocEntity = locEntity;
    }
}

public class RecruitUnitEventSystem : IEcsSystem
{
    Node3D _parent;

    public RecruitUnitEventSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        if (Data.Instance.Units.Count == 0)
        {
            return;
        }

        world.ForEach((ref RecruitUnitEvent recruitEvent) =>
        {
            var scenario = world.GetResource<Scenario>();
            var sideEntity = scenario.GetCurrentSideEntity();

            ref var gold = ref sideEntity.Get<Gold>();

            var locEntity = recruitEvent.LocEntity;

            if (locEntity.Has<HasUnit>())
            {
                GD.PrintErr("Location already has a unit! Not recruiting: " + recruitEvent.UnitType.Name);
                return;
            }
            
            var freeCoords = locEntity.Get<Coords>();

            var terrainEntity = locEntity.Get<HasBaseTerrain>().Entity;
            ref var elevationOffset = ref terrainEntity.Get<ElevationOffset>();

            UnitType unitType = recruitEvent.UnitType;
            _parent.AddChild(unitType);
            UnitView unitView = unitType.UnitView;
            unitType.RemoveChild(unitView);
            _parent.AddChild(unitView);

            var unitEntity = UnitFactory.CreateFromUnitType(world, unitType, unitView);

            gold.Value -= unitType.Cost;

            unitType.QueueFree();

            var position = freeCoords.World();
            position.y = locEntity.Get<Elevation>().Height + elevationOffset.Value;

            unitView.Position = position;

            unitEntity.Add(new Side(recruitEvent.Side));
            unitEntity.Add(freeCoords);

            unitEntity.Get<Attribute<Moves>>().Empty();
            unitEntity.Get<Attribute<Actions>>().Empty();

            var canvas = world.GetResource<Canvas>();
            var canvasLayer = canvas.GetCanvasLayer(0);

            var unitPlate = UnitPlate.Instantiate();
            
            canvasLayer.AddChild(unitPlate);

            unitEntity.Add(new NodeHandle<UnitPlate>(unitPlate));

            recruitEvent.LocEntity.Add(new HasUnit(unitEntity));
        });
    }
}
