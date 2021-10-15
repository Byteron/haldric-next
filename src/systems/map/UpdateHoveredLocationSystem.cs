using Godot;
using Godot.Collections;
using Bitron.Ecs;

public class UpdateHoveredLocationSystem : IEcsSystem
{
    private Node3D _parent;

    private Vector3 previousCell = Vector3.Zero;

    public UpdateHoveredLocationSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        var cursorQuery = world.Query<HoveredLocation>().End();

        if (!world.HasResource<Map>())
        {
            return;
        }

        var map = world.GetResource<Map>();

        ref var locations = ref map.Locations;

        var result = ShootRay();

        if (result.Contains("position"))
        {
            var position = (Vector3)result["position"];
            var coords = Coords.FromWorld(position);

            if (previousCell != coords.Axial)
            {
                foreach (var cursorEntityId in cursorQuery)
                {
                    var locEntity = locations.Get(coords.Cube);
                    ref var hoveredLocation = ref cursorQuery.Get<HoveredLocation>(cursorEntityId);
                    hoveredLocation.Entity = locEntity;
                    hoveredLocation.HasChanged = true;
                }

                previousCell = coords.Axial;
            }
        }
    }

    private Dictionary ShootRay()
    {
        var spaceState = _parent.GetWorld3d().DirectSpaceState;
        var viewport = _parent.GetViewport();

        var camera = viewport.GetCamera3d();
        var mousePosition = viewport.GetMousePosition();

        if (camera == null)
        {
            return new Dictionary();
        }
        
        var from = camera.ProjectRayOrigin(mousePosition);
        var to = from + camera.ProjectRayNormal(mousePosition) * 1000f;

        return spaceState.IntersectRay(from, to);
    }
}