using Godot;
using Bitron.Ecs;

public class UpdateMapCursorSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.HasResource<HoveredLocation>())
        {
            return;
        }

        var hoveredLocation = world.GetResource<HoveredLocation>();
        var cursor = world.GetResource<Cursor3D>();

        var locEntity = hoveredLocation.Entity;

        if (!locEntity.IsAlive())
        {
            return;
        }

        if (!hoveredLocation.HasChanged)
        {
            return;
        }

        hoveredLocation.HasChanged = false;
        
        ref var coords = ref locEntity.Get<Coords>();
        var height = locEntity.Get<Elevation>().Height;

        var position = coords.World;
        position.y = height;

        cursor.Position = position;
    }
}