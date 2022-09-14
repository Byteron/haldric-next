using Godot;
using System;
using RelEcs;

public partial class FloatingLabel : Control
{
    public static FloatingLabel Instantiate(Vector3 position, string text, Color color)
    {
        var label = Scenes.Instantiate<FloatingLabel>();
        label.WorldPosition = position;
        label.Text = text;
        label.Color = color;
        return label;
    }
    
    public Vector3 WorldPosition { get; set; }
    public Color Color { get; set; }
    public string Text { get; set; }

     Label _label;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");

        _label.Text = Text;
        _label.Modulate = Color;

        var tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.Out);
        tween.SetTrans(Tween.TransitionType.Back);
        tween.TweenProperty(this, "WorldPosition", WorldPosition + Vector3.Up * 6f, 0.5f);
        tween.TweenCallback(new Callable(this, "queue_free"));
        tween.Play();
    }

    public override void _Process(double delta)
    {
        var camera = GetViewport().GetCamera3d();

        if (camera == null)
        {
            QueueFree();
            return;
        }
        
        if (camera.IsPositionBehind(WorldPosition))
        {
            Hide();
        }
        else
        {
            Show();
            Position = camera.UnprojectPosition(WorldPosition);
        }
    }
}
