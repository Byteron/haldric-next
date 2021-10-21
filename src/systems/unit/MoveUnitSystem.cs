using Godot;
using Bitron.Ecs;

public class MoveUnitSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var cursorQuery = world.Query<HoveredLocation>().Inc<HasLocation>().End();

        var commander = world.GetResource<Commander>();
        
        foreach(var cursorEntityId in cursorQuery)
        {
            var map = world.GetResource<Map>();
            
            var hoveredLocEntity = cursorQuery.Get<HoveredLocation>(cursorEntityId).Entity;
            
            if (!hoveredLocEntity.IsAlive())
            {
                return;
            }

            ref var hasLocation = ref cursorQuery.Get<HasLocation>(cursorEntityId);
            var selectedLocEntity = hasLocation.Entity;

            if (Input.IsActionJustPressed("select_unit"))
            {
                ref var startCoords = ref selectedLocEntity.Get<Coords>();
                ref var targetCoords = ref hoveredLocEntity.Get<Coords>();

                if (startCoords.Cube == targetCoords.Cube)
                {
                    return;
                }
                
                var path = map.FindPath(startCoords, targetCoords);

                var unitEntity = selectedLocEntity.Get<HasUnit>().Entity;
                ref var moves = ref unitEntity.Get<Attribute<Moves>>();

                if (path.Checkpoints.Count > moves.Value)
                {
                    return;
                }

                commander.Enqueue(new MoveUnitCommand(path));
                hasLocation.Entity = hoveredLocEntity;
                world.GetResource<GameStateController>().PushState(new CommanderState(world));
            }
        }
    }
}