using Godot;
using RelEcs;
using RelEcs.Godot;

public class UpdateCameraOperatorSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<CameraOperator>(out var cameraOperator))
        {
            return;
        }

        if (!commands.HasElement<Map>())
        {
            return;
        }

        var map = commands.GetElement<Map>();

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

        cameraOperator.MinimumPosition = map.GetBeginPosition();
        cameraOperator.MaximumPosition = map.GetEndPosition();

        cameraOperator.UpdatePosition(direction);
        cameraOperator.UpdateRotation();
        cameraOperator.UpdateZoom();
    }

     Vector3 GetWalkInput()
    {
        int left = Input.IsActionPressed("camera_left") ? 1 : 0;
        int right = Input.IsActionPressed("camera_right") ? 1 : 0;
        int forward = Input.IsActionPressed("camera_forward") ? 1 : 0;
        int back = Input.IsActionPressed("camera_back") ? 1 : 0;

        return new Vector3(right - left, 0, back - forward).Normalized();
    }
}