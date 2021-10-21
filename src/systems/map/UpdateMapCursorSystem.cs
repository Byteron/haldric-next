using Godot;
using Bitron.Ecs;

public class UpdateMapCursorSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var hoverQuery = world.Query<HoveredLocation>()
            .Inc<Highlighter>()
            .Inc<NodeHandle<Cursor3D>>()
            .End();
        
        foreach(var cursorEntityId in hoverQuery)
        {
            ref var hoveredLocation = ref hoverQuery.Get<HoveredLocation>(cursorEntityId);
            var locEntity = hoveredLocation.Entity;

            if (!locEntity.IsAlive())
            {
                continue;
            }

            if (!hoveredLocation.HasChanged)
            {
                return;
            }

            hoveredLocation.HasChanged = false;
            
            var view = hoverQuery.Get<NodeHandle<Cursor3D>>(cursorEntityId).Node;

            ref var coords = ref locEntity.Get<Coords>();
            var height = locEntity.Get<Elevation>().Height;

            var position = coords.World;
            position.y = height;

            view.Position = position;
        }
        
    }
}