using Godot;

namespace Haldric;

public partial class CameraOperator : Node3D
{
    float _zoom = 0.5f;
    float _targetZoom = 0.5f;

    float[] _rotations = new float[4] { 0f, 90f, 180f, -90f };
    int _rotationIndex = 0;
    float _targetRotation = 0f;

    // Vector3 _minimumPosition = Vector3.Zero;
    // Vector3 _maximumPosition = Vector3.Zero;

    [Export] Vector3 _cameraOffset = Vector3.Up;
    [Export] float _walkSpeed = 30f;
    [Export] float _minDistance = 60f;
    [Export] float _maxDistance = 60f;
    [Export] Curve _zoomCurve = default!;

    [Export] Node3D _gimbalH = default!;
    [Export] Node3D _gimbalV = default!;
    [Export] Camera3D _camera = default!;

    public override void _Ready()
    {
        _gimbalH.Position = _cameraOffset;
    }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("camera_zoom_out"))
        {
            _targetZoom = Mathf.Clamp(_targetZoom - 0.05f, 0, 1);
        }

        if (e.IsActionPressed("camera_zoom_in"))
        {
            _targetZoom = Mathf.Clamp(_targetZoom + 0.05f, 0, 1);
        }
    }

    public override void _Process(double delta)
    {
        // Position
        var left = Input.IsActionPressed("camera_left") ? 1 : 0;
        var right = Input.IsActionPressed("camera_right") ? 1 : 0;
        var forward = Input.IsActionPressed("camera_forward") ? 1 : 0;
        var back = Input.IsActionPressed("camera_back") ? 1 : 0;

        var inputDirection = new Vector3(right - left, 0, back - forward).Normalized();
        ;
        var direction = inputDirection.Rotated(new Vector3(0, 1, 0), Rotation.Y);

        var transform = Transform;
        transform.Origin = Transform.Origin + direction * _walkSpeed * (float)GetProcessDeltaTime();
        // transform.Origin.X = Mathf.Max(transform.Origin.X, _minimumPosition.X);
        // transform.Origin.Z = Mathf.Max(transform.Origin.Z, _minimumPosition.Z);
        // transform.Origin.X = Mathf.Min(transform.Origin.X, _maximumPosition.X);
        // transform.Origin.Z = Mathf.Min(transform.Origin.Z, _maximumPosition.Z);
        Transform = transform;

        // Rotation
        if (Input.IsActionJustPressed("camera_turn_left"))
        {
            _rotationIndex = (_rotationIndex + 1) % _rotations.Length;
            _targetRotation = _rotations[_rotationIndex];
        }

        if (Input.IsActionJustPressed("camera_turn_right"))
        {
            _rotationIndex -= 1;
            _rotationIndex = _rotationIndex == -1 ? _rotationIndex + _rotations.Length : _rotationIndex;
            _targetRotation = _rotations[_rotationIndex];
        }

        _camera.Position = new Vector3(0, 0, Mathf.Lerp(_minDistance, _maxDistance, _zoom));

        var rotation = Rotation;
        rotation.Y = Mathf.LerpAngle(Rotation.Y, Mathf.DegToRad(_targetRotation), 0.08f);
        Rotation = rotation;

        // Zoom
        _zoom = Mathf.Lerp(_zoom, _targetZoom, 0.1f);

        var gimbalVRotation = _gimbalV.Rotation;
        gimbalVRotation.X = Mathf.DegToRad(Mathf.Lerp(0, -90, _zoomCurve.Sample(_zoom)));
        _gimbalV.Rotation = gimbalVRotation;
    }

    public void Focus(Vector3 worldPosition)
    {
        var tween = GetTree().CreateTween();

        tween.SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Sine)
            .TweenProperty(this, "position", worldPosition, 0.25f);

        tween.Play();
    }
}