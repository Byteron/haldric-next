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

        ref var targetCoords = ref ToLocEntity.Get<Coords>();
        ref var targetElevation = ref ToLocEntity.Get<Elevation>();

        var newPos = targetCoords.World;
        newPos.y = targetElevation.Height;

        unitView.Position = newPos;

        FromLocEntity.Remove<HasUnit>();
        ToLocEntity.Add(new HasUnit(unitEntity));
    }

    public override void Revert()
    {
        var temp = FromLocEntity;
        FromLocEntity = ToLocEntity;
        ToLocEntity = temp;

        IsReverted = true;
    }
}