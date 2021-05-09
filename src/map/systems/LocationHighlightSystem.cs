using Godot;
using Leopotam.Ecs;

struct Highlighter {}

public class LocationHighlightSystem : IEcsInitSystem, IEcsRunSystem
{
    Node3D _parent;

    EcsWorld _world;
    EcsFilter<HoveredCoords> _hoveredCoords;
    EcsFilter<Highlighter> _highlighter;
    EcsFilter<Locations, Map> _maps;

    public LocationHighlightSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Init()
    {
        var entity = _world.NewEntity();

        var view = Scenes.Instance.LocationHighlight.Instance<Node3D>();

        _parent.AddChild(view);

        entity.Replace(new NodeHandle<Node3D>(view));
        entity.Get<Highlighter>();
    }

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