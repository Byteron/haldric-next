using Bitron.Ecs;

public class MoveUnitCommand : Command
{
    public EcsEntity FromLocEntity;
    public EcsEntity ToLocEntity;

    public MoveUnitCommand(EcsEntity from, EcsEntity to)
    {
        IsRevertable = true;

        FromLocEntity = from;
        ToLocEntity = to;
    }

    public override void Execute()
    {
        // source location does not have a unit to move
        if (!FromLocEntity.Has<HasUnit>())
        {
            return;
        }

        // target loc already occupied
        if (ToLocEntity.Has<HasUnit>())
        {
            return;
        }

        var unitEntity = FromLocEntity.Get<HasUnit>().Entity;
        var unitView = unitEntity.Get<NodeHandle<UnitView>>().Node;
        
        ref var unitCoords = ref unitEntity.Get<Coords>();
        ref var unitMoves = ref unitEntity.Get<Attribute<Moves>>();

        ref var targetCoords = ref ToLocEntity.Get<Coords>();
        ref var targetElevation = ref ToLocEntity.Get<Elevation>();

        var newPos = targetCoords.World;
        newPos.y = targetElevation.Height;

        unitView.Position = newPos;

        FromLocEntity.Remove<HasUnit>();
        ToLocEntity.Add(new HasUnit(unitEntity));
        
        unitCoords = targetCoords;
        unitMoves.Empty();

        Main.Instance.World.Spawn().Add(new UnitSelectedEvent(unitEntity));
    }

    public override void Revert()
    {
        var temp = FromLocEntity;
        FromLocEntity = ToLocEntity;
        ToLocEntity = temp;

        IsReverted = true;
    }
}