using RelEcs;
using Godot;

public class UpdateCameraSystem : ISystem
{
    public World World { get; set; }

    public void Run(World world)
    {
        if (!world.TryGetElement<CameraOperator>(out var cameraOperator)) return;

        if (Input.IsActionPressed("camera_zoom_out"))
        {
            cameraOperator.ZoomOut();
        }

        if (Input.IsActionPressed("camera_zoom_in"))
        {
            cameraOperator.ZoomIn();
        }

        if (Input.IsActionJustPressed("camera_turn_left"))
        {
            cameraOperator.TurnLeft();
        }

        if (Input.IsActionJustPressed("camera_turn_right"))
        {
            cameraOperator.TurnRight();
        }

        var rawDirection = GetWalkInput();
        var direction = cameraOperator.GetRelativeWalkInput(rawDirection);

        cameraOperator.UpdatePosition(direction);
        cameraOperator.UpdateRotation();
        cameraOperator.UpdateZoom();
        
        if (!world.TryGetElement<Map>(out var map)) return;

        cameraOperator.MinimumPosition = Map.GetBeginPosition();
        cameraOperator.MaximumPosition = map.GetEndPosition();
    }

    static Vector3 GetWalkInput()
    {
        var left = Input.IsActionPressed("camera_left") ? 1 : 0;
        var right = Input.IsActionPressed("camera_right") ? 1 : 0;
        var forward = Input.IsActionPressed("camera_forward") ? 1 : 0;
        var back = Input.IsActionPressed("camera_back") ? 1 : 0;

        return new Vector3(right - left, 0, back - forward).Normalized();
    }
}