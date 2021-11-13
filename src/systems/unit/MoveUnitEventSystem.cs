using Bitron.Ecs;
using Godot;

public struct MoveUnitEvent
{
    public Vector3 From;
    public Vector3 To;
}

public class MoveUnitEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<MoveUnitEvent>().End();

        foreach (var eventId in query)
        {
            var map = world.GetResource<Map>();
            var commander = world.GetResource<Commander>();

            var eventEntity = world.Entity(eventId);
            ref var moveEvent = ref eventEntity.Get<MoveUnitEvent>();

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

            if (path.Checkpoints.Count > moves.Value)
            {
                return;
            }

            commander.Enqueue(new MoveUnitCommand(path));
            world.GetResource<GameStateController>().PushState(new CommanderState(world));
        }
    }
}