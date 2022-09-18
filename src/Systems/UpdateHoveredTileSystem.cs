using Godot;
using Godot.Collections;
using RelEcs;

public class UpdateHoveredTileSystem : ISystem
{
    public World World { get; set; }

    Vector3 _previousCell = Vector3.Zero;

    public void Run()
    {
        if (!this.HasElement<Map>()) return;
        if (!this.TryGetElement<HoveredTile>(out var hoveredTile)) return;
        
        var result = ShootRay();

        if (!result.ContainsKey("position")) return;

        var position = (Vector3)result["position"];
        var coords = Coords.FromWorld(position);

        if (_previousCell == coords.ToAxial()) return;

        var map = this.GetElement<Map>();

        var tileEntity = map.Tiles.Get(coords.ToCube());

        hoveredTile.Entity = tileEntity;
        hoveredTile.HasChanged = true;
        
        GD.Print(coords);

        _previousCell = coords.ToAxial();
    }

    Dictionary ShootRay()
    {
        
        var scene = (Node3D)this.GetTree().CurrentScene;
        var spaceState = scene.GetWorld3d().DirectSpaceState;
        var viewport = scene.GetViewport();

        var camera = viewport.GetCamera3d();
        var mousePosition = viewport.GetMousePosition();

        if (camera == null) return new Dictionary();

        var from = camera.ProjectRayOrigin(mousePosition);
        var to = from + camera.ProjectRayNormal(mousePosition) * 1000f;

        var parameters3D = new PhysicsRayQueryParameters3D();
        parameters3D.From = from;
        parameters3D.To = to;

        return spaceState.IntersectRay(parameters3D);
    }
}