using Godot;
using RelEcs;
using RelEcs.Godot;

public class UpdateMapCursorSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        if (!commands.TryGetElement<Cursor3D>(out var cursor))
        {
            return;
        }

        var locEntity = hoveredLocation.Entity;

        if (!locEntity.IsAlive)
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