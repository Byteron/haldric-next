using Godot;

public partial class CameraOperator : Node3D
{
    float _zoom = 0.5f;
    float _targetZoom = 0.5f;

    float[] _rotations = new float[4] { 0f, 90f, 180f, -90f };
    int _rotationIndex = 0;
    float _targetRotation = 0f;

    [Export] Vector3 _cameraOffset = Vector3.Up;
    [Export] float _walkSpeed = 30f;
    [Export] float _minDistance = 60f;
    [Export] float _maxDistance = 60f;
    [Export] Curve _zoomCurve;

    Node3D _gimbalH;
    Node3D _gimbalV;
    Camera3D _camera;

    public Vector3 MinimumPosition { get; set; }
    public Vector3 MaximumPosition { get; set; }

    public Camera3D Camera => _camera;


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

    public void Focus(Vector3 worldPosition)
    {
        var tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Sine)
            .TweenProperty(this, "position", worldPosition, 0.25f);

        tween.Play();
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
        transform.Origin = Transform.Origin + direction * _walkSpeed * (float)GetProcessDeltaTime();
        transform.Origin.X = Mathf.Max(transform.Origin.X, MinimumPosition.X);
        transform.Origin.Z = Mathf.Max(transform.Origin.Z, MinimumPosition.Z);
        transform.Origin.X = Mathf.Min(transform.Origin.X, MaximumPosition.X);
        transform.Origin.Z = Mathf.Min(transform.Origin.Z, MaximumPosition.Z);
        Transform = transform;
    }

    public void UpdateRotation()
    {
        _camera.Position = new Vector3(0, 0, Mathf.Lerp(_minDistance, _maxDistance, _zoom));

        var rotation = Rotation;
        rotation.Y = Mathf.LerpAngle(Rotation.Y, Mathf.DegToRad(_targetRotation), 0.08f);
        Rotation = rotation;
    }

    public void UpdateZoom()
    {
        _zoom = Mathf.Lerp(_zoom, _targetZoom, 0.1f);

        var gimbalVRotation = _gimbalV.Rotation;
        gimbalVRotation.X = Mathf.DegToRad(Mathf.Lerp(0, -90, _zoomCurve.Sample(_zoom)));
        _gimbalV.Rotation = gimbalVRotation;
    }

    public Vector3 GetRelativeWalkInput(Vector3 walkInput)
    {
        return walkInput.Rotated(new Vector3(0, 1, 0), Rotation.Y);
    }
}