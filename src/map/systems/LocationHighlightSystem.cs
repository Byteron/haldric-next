using Godot;
using Leopotam.Ecs;

struct Highlighter {}

public class LocationHighlightSystem : IEcsRunSystem
{
    EcsFilter<HoveredCoords> _hoveredCoords;
    EcsFilter<Highlighter> _highlighter;
    EcsFilter<Locations, Map> _maps;

    public void Run()
    {
        if (_highlighter.IsEmpty() || _hoveredCoords.IsEmpty() || _maps.IsEmpty())
        {
            return;
        }

        var coords = _hoveredCoords.GetEntity(0).Get<HoveredCoords>().Coords;
        var location = _maps.GetEntity(0).Get<Locations>().Get(coords.Cube);
        var height = location.Get<Elevation>().Height;
        var view = _highlighter.GetEntity(0).Get<NodeHandle<Node3D>>().Node;

        var position = coords.World;
        position.y = height;

        GD.Print(coords.ToString());

        view.Translation = position;
    }
}