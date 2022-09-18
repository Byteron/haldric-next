using Godot;
using RelEcs;

public class UpdateMapCursorSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        if (!this.TryGetElement<HoveredTile>(out var hoveredTile)) return;
        if (!this.TryGetElement<Cursor3D>(out var cursor)) return;
        if (!this.IsAlive(hoveredTile.Entity)) return;
        if (!hoveredTile.HasChanged) return;

        hoveredTile.HasChanged = false;

        var tiles = this.Query<Coords, Elevation>();
        var (coords, elevation) = tiles.Get(hoveredTile.Entity);

        var position = coords.ToWorld();
        position.y = elevation.Height;

        cursor.Position = position;
    }
}