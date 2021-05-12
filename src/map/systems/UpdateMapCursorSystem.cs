using Godot;
using Godot.Collections;
using Leopotam.Ecs;

public class UpdateMapCursorSystem : IEcsRunSystem
{
    private EcsFilter<HoveredLocation> _hoveredLocation;
    private EcsFilter<Locations, Map> _locations;

    private Node3D _parent;

    private Vector3 previousCell = Vector3.Zero;

    public UpdateMapCursorSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run()
    {
        if (_locations.IsEmpty())
        {
            return;
        }

        ref var locations = ref _locations.GetEntity(0).Get<Locations>();

        var result = ShootRay();

        if (result.Contains("position"))
        {
            var position = (Vector3)result["position"];
            var coords = Coords.FromWorld(position);

            if (previousCell != coords.Axial)
            {
                foreach(var i in _hoveredLocation)
                {
                    var locEntity = locations.Get(coords.Cube);
                    _hoveredLocation.GetEntity(i).Get<HoveredLocation>().Entity = locEntity;
                }

                previousCell = coords.Axial;
            }

        }
    }

    private Dictionary ShootRay()
    {
        var spaceState = _parent.GetWorld3d().DirectSpaceState;
        var viewport = _parent.GetViewport();

        var camera = viewport.GetCamera();
        var mousePosition = viewport.GetMousePosition();

        var from = camera.ProjectRayOrigin(mousePosition);
        var to = from + camera.ProjectRayNormal(mousePosition) * 1000f;

        return spaceState.IntersectRay(from, to);
    }
}