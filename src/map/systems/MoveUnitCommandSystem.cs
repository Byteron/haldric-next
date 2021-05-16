using Leopotam.Ecs;

public class MoveCommand : Command
{
    public EcsEntity FromLocEntity;
    public EcsEntity ToLocEntity;

    public MoveCommand(EcsEntity from, EcsEntity to)
    {
        IsRevertable = true;

        FromLocEntity = from;
        ToLocEntity = to;
    }

    public override void Revert()
    {
        var temp = FromLocEntity;
        FromLocEntity = ToLocEntity;
        ToLocEntity = temp;

        IsReverted = true;
    }
}

public class MoveUnitCommandSystem : IEcsRunSystem
{
    EcsFilter<Commander> _filter;

    public void Run()
    {
        foreach (var i in _filter)
        {
            var entity = _filter.GetEntity(i);
            ref var commander = ref entity.Get<Commander>();

            if (commander.IsEmpty())
            {
                return;
            }

            // only process move commands
            if (!(commander.Peek() is MoveCommand))
            {
                return;
            }

            var command = commander.Dequeue() as MoveCommand;

            // source location does not have a unit to move
            if (!command.FromLocEntity.Has<HasUnit>())
            {
                return;
            }

            // target loc already occupied
            if (command.ToLocEntity.Has<HasUnit>())
            {
                return;
            }

            var unitEntity = command.FromLocEntity.Get<HasUnit>().Entity;
            var unitView = unitEntity.Get<NodeHandle<UnitView>>().Node;

            ref var targetCoords = ref command.ToLocEntity.Get<Coords>();
            ref var targetElevation = ref command.ToLocEntity.Get<Elevation>();

            var newPos = targetCoords.World;
            newPos.y = targetElevation.Height;

            unitView.Translation = newPos;

            command.FromLocEntity.Del<HasUnit>();
            command.ToLocEntity.Replace(new HasUnit(unitEntity));
        }

    }
}