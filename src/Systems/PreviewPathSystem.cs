using Godot;
using RelEcs;
using RelEcs.Godot;

public class PreviewPathSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation)) return;
        if (!commands.TryGetElement<SelectedLocation>(out var selectedLocation)) return;

        var hLocEntity = hoveredLocation.Entity;
        var sLocEntity = selectedLocation.Entity;

        if (!hLocEntity.IsAlive) return;
        if (!sLocEntity.IsAlive) return;
        if (!hoveredLocation.HasChanged) return;

        var highlighter = commands.GetElement<TerrainHighlighter>();
        highlighter.ClearPath();

        if (hLocEntity.Has<HasUnit>()) return;

        var map = commands.GetElement<Map>();
        var scenario = commands.GetElement<Scenario>();

        var path = map.FindPath(sLocEntity.Get<Coords>(), hLocEntity.Get<Coords>(), scenario.Side);
        highlighter.ShowPath(path);

    }
}