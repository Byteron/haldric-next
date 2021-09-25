using Godot;

public partial class CameraOperator : Node3D
{
    private float _zoom = 0.5f;
    private float _targetZoom = 0.5f;

    private float[] _rotations = new float[4] {0f, 90f, 180f, -90f};
    private int _rotationIndex = 0;
    private float _targetRotation = 0f;
    
    [Export] private Vector3 _cameraOffset = Vector3.Up;
    [Export] private float _walkSpeed = 30f;
    [Export] private float _maxDistance = 60f;
    [Export] private Curve _zoomCurve;

    private Node3D _gimbalH;
    private Node3D _gimbalV;
    private Camera3D _camera;


    public override void _Ready()
    {
        _gimbalH = GetNode<Node3D>("HorizontalGimbal");
        _gimbalV = GetNode<Node3D>("HorizontalGimbal/VerticalGimbal");
        _camera = GetNode<Camera3D>("HorizontalGimbal/VerticalGimbal/Camera");

        _gimbalH.Position = _cameraOffset;
    }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("camera_zoom_out"))
        {
            ZoomOut();
        }

        if (e.IsActionPressed("camera_zoom_in"))
        {
            ZoomIn();
        }
    }

    public void ZoomIn()
    {
        _targetZoom = Mathf.Clamp(_targetZoom + 0.05f, 0, 1);
    }

    public void ZoomOut()
    {
        _targetZoom = Mathf.Clamp(_targetZoom - 0.05f, 0, 1);
    }

    public void TurnLeft()
    {
        _rotationIndex = (_rotationIndex + 1) % _rotations.Length;
        _targetRotation = _rotations[_rotationIndex];
    }

    public void TurnRight()
    {
        _rotationIndex -= 1;
        _rotationIndex = _rotationIndex == -1 ? _rotationIndex + _rotations.Length : _rotationIndex;
        _targetRotation = _rotations[_rotationIndex];
    }

    public void UpdatePosition(Vector3 direction)
    {
        
        var transform = Transform;
        transform.origin = Transform.origin + direction * _walkSpeed * (float) GetProcessDeltaTime();
        Transform = transform;
    }

    public void UpdateRotation()
    {
        _camera.Position = new Vector3(0, 0, Mathf.Lerp(0, _maxDistance, _zoom));

        var rotation = Rotation;
        rotation.y = Mathf.LerpAngle(Rotation.y, Mathf.Deg2Rad(_targetRotation), 0.08f);
        Rotation = rotation;
    }

    public void UpdateZoom()
    {
        _zoom = Mathf.Lerp(_zoom, _targetZoom, 0.1f);

        var gimbalVRotation = _gimbalV.Rotation;
        gimbalVRotation.x = Mathf.Deg2Rad(Mathf.Lerp(0, -90, _zoomCurve.Interpolate(_zoom)));
        _gimbalV.Rotation = gimbalVRotation;
    }

    public Vector3 GetRelativeWalkInput(Vector3 walkInput)
    {
        return walkInput.Rotated(new Vector3(0, 1, 0), Rotation.y);
    }
}
