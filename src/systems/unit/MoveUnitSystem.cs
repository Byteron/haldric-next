using Godot;
using Bitron.Ecs;

public class MoveUnitSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {   
        if (!world.TryGetResource<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        if (!world.TryGetResource<SelectedLocation>(out var selectedLocation))
        {
            return;
        }

        var commander = world.GetResource<Commander>();
        
        var map = world.GetResource<Map>();
        
        var hoveredLocEntity = hoveredLocation.Entity;
        
        if (!hoveredLocEntity.IsAlive() || hoveredLocEntity.Has<HasUnit>())
        {
            return;
        }

        var selectedLocEntity = selectedLocation.Entity;

        if (selectedLocEntity.Get<Coords>().Cube == hoveredLocEntity.Get<Coords>().Cube)
        {
            return;
        }

        if (Input.IsActionJustPressed("select_unit"))
        {
            ref var startCoords = ref selectedLocEntity.Get<Coords>();
            ref var targetCoords = ref hoveredLocEntity.Get<Coords>();

            if (startCoords.Cube == targetCoords.Cube)
            {
                return;
            }
            
            var unitEntity = selectedLocEntity.Get<HasUnit>().Entity;

            var path = map.FindPath(startCoords, targetCoords, unitEntity.Get<Side>().Value);

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
            selectedLocation.Entity = hoveredLocEntity;
            world.GetResource<GameStateController>().PushState(new CommanderState(world));
        }
    }
}