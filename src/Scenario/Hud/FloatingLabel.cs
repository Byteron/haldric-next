using System;
using Godot;

namespace Haldric;

public partial class FloatingLabel : Control
{
    public static FloatingLabel Instantiate(Vector3 position, string text, Color color)
    {
        var label = GD.Load<PackedScene>("src/Scenario/Hud/FloatingLabel.tscn").Instantiate<FloatingLabel>();
        label._worldPosition = position;
        label._text = text;
        label._color = color;
        return label;
    }

    Vector3 _worldPosition;
    Color _color;
    string _text = string.Empty;

    [Export] Label _label = default!;

    public override void _Ready()
    {
        _label.Text = _text;
        _label.Modulate = _color;

        var tween = GetTree().CreateTween();

        tween
            .SetParallel(false)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Back)
            .TweenProperty(this, "_worldPosition", _worldPosition + Vector3.Up * 6f, 0.5f);

        tween.TweenCallback(new Callable(this, "queue_free"));

        tween.Play();
    }

    public override void _Process(double delta)
    {
        var camera = GetViewport().GetCamera3D();

        if (camera == null)
        {
            QueueFree();
            return;
        }

        if (camera.IsPositionBehind(_worldPosition))
        {
            Hide();
        }
        else
        {
            Show();
            Position = camera.UnprojectPosition(_worldPosition);
        }
    }
}