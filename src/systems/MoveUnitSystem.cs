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
            var hoveredLocEntity = cursorQuery.Get<HoveredLocation>(cursorEntityId).Entity;
            
            if (!hoveredLocEntity.IsAlive())
            {
                return;
            }

            ref var hasLocation = ref cursorQuery.Get<HasLocation>(cursorEntityId);
            var selectedLocEntity = hasLocation.Entity;

            if (Input.IsActionJustPressed("select_unit"))
            {
                commander.Enqueue(new MoveCommand(selectedLocEntity, hoveredLocEntity));
                hasLocation.Entity = hoveredLocEntity;
            }
        }
    }
}