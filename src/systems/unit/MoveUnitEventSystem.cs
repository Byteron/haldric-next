using RelEcs;
using RelEcs.Godot;
using Godot;

public class MoveUnitEvent
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

            var fromCoords = fromLocEntity.Get<Coords>();
            var toCoords = toLocEntity.Get<Coords>();

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

            var moves = unitEntity.Get<Attribute<Moves>>();

            if (path.GetCost() > moves.Value)
            {
                return;
            }

            commander.Enqueue(new MoveUnitCommand(path));
            commands.GetElement<GameStateController>().PushState(new CommanderState());
        });
    }
}