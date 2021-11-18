using Godot;
using Bitron.Ecs;

public class UpdateMapCursorSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.TryGetResource<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        if (!world.TryGetResource<Cursor3D>(out var cursor))
        {
            return;
        }

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

        var position = coords.World();
        position.y = height;

        cursor.Position = position;
    }
}