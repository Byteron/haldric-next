using Godot;
using Bitron.Ecs;

public class PreviewPathSystem : IEcsSystem
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

        var hLocEntity = hoveredLocation.Entity;
        var sLocEntity = selectedLocation.Entity;

        if (!hLocEntity.IsAlive())
        {
            return;
        }

        if (!sLocEntity.IsAlive())
        {
            return;
        }

        if (!hoveredLocation.HasChanged)
        {
            return;
        }

        var highlighter = world.GetResource<TerrainHighlighter>();
        highlighter.ClearPath();
        
        if (hLocEntity.Has<HasUnit>())
        {
            return;
        }

        var map = world.GetResource<Map>();
        var scenario = world.GetResource<Scenario>();

        Path path = map.FindPath(sLocEntity.Get<Coords>(), hLocEntity.Get<Coords>(), scenario.CurrentPlayer);
        highlighter.ShowPath(path);

    }
}