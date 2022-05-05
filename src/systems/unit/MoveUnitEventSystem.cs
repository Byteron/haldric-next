using RelEcs;
using RelEcs.Godot;
using Godot;

public struct MoveUnitEvent
{
    public Vector3 From;
    public Vector3 To;
}

public class MoveUnitEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((MoveUnitEvent moveEvent) =>
        {
            var map = commands.GetElement<Map>();
            var commander = commands.GetElement<Commander>();


            var fromLocEntity = map.Locations.Get(moveEvent.From);
            var toLocEntity = map.Locations.Get(moveEvent.To);

            ref var fromCoords = ref fromLocEntity.Get<Coords>();
            ref var toCoords = ref toLocEntity.Get<Coords>();

            if (fromCoords.Cube() == toCoords.Cube())
            {
                return;
            }

            var unitEntity = fromLocEntity.Get<HasUnit>().Entity;

            var path = map.FindPath(fromCoords, toCoords, unitEntity.Get<Side>().Value);

            if (path.Checkpoints.Count == 0)
            {
                return;
            }

            ref var moves = ref unitEntity.Get<Attribute<Moves>>();

            if (path.GetCost() > moves.Value)
            {
                return;
            }

            commander.Enqueue(new MoveUnitCommand(path));
            commands.GetElement<GameStateController>().PushState(new CommanderState());
        });
    }
}