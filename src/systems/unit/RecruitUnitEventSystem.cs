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

        var castleQuery = world.Query<Castle>().End();
        var eventQuery = world.Query<RecruitUnitEvent>().End();

        foreach (var eventEntityId in eventQuery)
        {
            var scenario = world.GetResource<Scenario>();
            var player = scenario.GetCurrentPlayerEntity();

            ref var gold = ref player.Get<Gold>();

            ref var recruitEvent = ref world.Entity(eventEntityId).Get<RecruitUnitEvent>();

            var locEntity = recruitEvent.LocEntity;
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

            var hudView = world.GetResource<HUDView>();

            unitEntity.Add(new NodeHandle<UnitPlate>(hudView.CreateUnitPlate()));

            recruitEvent.LocEntity.Add(new HasUnit(unitEntity));
        }
    }
}
