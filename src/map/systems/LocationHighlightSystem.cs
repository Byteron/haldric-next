using Godot;
using Leopotam.Ecs;

struct Highlighter {}

public class LocationHighlightSystem : IEcsRunSystem
{
    EcsFilter<HoveredLocation> _hoveredLocations;
    EcsFilter<Highlighter> _highlighter;
    EcsFilter<Locations, Map> _maps;

    public void Run()
    {
        if (_highlighter.IsEmpty() || _hoveredLocations.IsEmpty() || _maps.IsEmpty())
        {
            return;
        }

        var locEntity = _hoveredLocations.GetEntity(0).Get<HoveredLocation>().Entity;

        if (locEntity == EcsEntity.Null)
        {
            return;
        }
        
        ref var coords = ref locEntity.Get<Coords>();
        var height = locEntity.Get<Elevation>().Height;
        var view = _highlighter.GetEntity(0).Get<NodeHandle<Node3D>>().Node;

        var position = coords.World;
        position.y = height;

        GD.Print(coords.ToString());

        view.Position = position;
    }
}