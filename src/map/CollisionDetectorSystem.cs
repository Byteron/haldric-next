using Godot;
using Godot.Collections;
using Leopotam.Ecs;

public class CollisionDetectorSystem : IEcsRunSystem
{
    private EcsFilter<HoveredCoords> _filter;

    private Node3D _parent;

    private Vector3 previousCell = Vector3.Zero;

    public CollisionDetectorSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run()
    {
        var result = ShootRay();

        if (result.Contains("position"))
        {
            var position = (Vector3)result["position"];
            var coords = Coords.FromWorld(position);

            if (previousCell != coords.Axial)
            {
                foreach (var i in _filter)
                {
                    _filter.GetEntity(i).Get<HoveredCoords>().Coords = coords;
                    GD.Print(coords);
                }

                previousCell = coords.Axial;
            }

        }
    }

    public Dictionary ShootRay()
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