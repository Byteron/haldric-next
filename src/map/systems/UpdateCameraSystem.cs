using Godot;
using Bitron.Ecs;

public class CameraOperatorSystem : IEcsSystem, IEcsInitSystem, IEcsDestroySystem
{
    EcsWorld _world;
    EcsFilter<NodeHandle<CameraOperator>> _filter;

    Node3D _parent;

    public CameraOperatorSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Init()
    {
        var cameraEntity = _world.NewEntity();

        var cameraOperator = Scenes.Instance.CameraOperator.Instantiate<CameraOperator>();
        _parent.AddChild(cameraOperator);

        cameraEntity.Replace(new NodeHandle<CameraOperator>(cameraOperator));
    }

    public void Destroy()
    {
        foreach (var i in _filter)
        {
            var entity = _filter.GetEntity(i);
            entity.Destroy();
        }
    }

    public void Run(EcsWorld world)
    {
        if (_filter.IsEmpty())
        {
            return;
        }

        foreach (var i in _filter)
        {
            var entity = _filter.GetEntity(i);

            CameraOperator cameraOperator = entity.Get<NodeHandle<CameraOperator>>().Node;

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