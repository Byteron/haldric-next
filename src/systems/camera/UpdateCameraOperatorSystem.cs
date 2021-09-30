using Godot;
using Bitron.Ecs;

public class UpdateCameraOperatorSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<NodeHandle<CameraOperator>>().End();

        foreach (var entityId in query)
        {
            CameraOperator cameraOperator = query.Get<NodeHandle<CameraOperator>>(entityId).Node;

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
        }
    }

    private Vector3 GetWalkInput()
    {
        int left = Input.IsActionPressed("camera_left") ? 1 : 0;
        int right = Input.IsActionPressed("camera_right") ? 1 : 0;
        int forward = Input.IsActionPressed("camera_forward") ? 1 : 0;
        int back = Input.IsActionPressed("camera_back") ? 1 : 0;

        return new Vector3(left - right, 0, forward - back).Normalized();
    }
}