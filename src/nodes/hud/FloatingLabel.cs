using Godot;
using System;
using Bitron.Ecs;

public partial class FloatingLabel : Control
{
    public Vector3 Position;
    public Color Color;
    public string Text;

    private Label _label;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");

        _label.Text = Text;
        _label.Modulate = Color;

        var tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.Out);
        tween.SetTrans(Tween.TransitionType.Back);
        tween.TweenProperty(this, "Position", Position + Vector3.Up * 6f, 0.5f);
        tween.TweenCallback(new Callable(this, "queue_free"));
        tween.Play();
    }

    public override void _Process(float delta)
    {
        var camera = GetViewport().GetCamera3d();

        if (camera.IsPositionBehind(Position))
        {
            Hide();
        }
        else
        {
            Show();
            RectPosition = camera.UnprojectPosition(Position);
        }
    }
}
