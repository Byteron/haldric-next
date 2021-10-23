using Godot;
using System;

public partial class UnitPlate : Control
{
    public Vector3 Position;

    public int MaxHealth;
    public int  Health;

    public int MaxActions;
    public int  Actions;

    public int MaxExperience;
    public int  Experience;

    public Color TeamColor;

    private TextureProgressBar _healthBar;
    private TextureProgressBar _xpBar;
    private ColorRect _rect;
    private Control _actionContainer;

    public override void _Ready()
    {
        _healthBar = GetNode<TextureProgressBar>("VBoxContainer/HealthProgressBar");
        _xpBar = GetNode<TextureProgressBar>("VBoxContainer/HBoxContainer/VBoxContainer/ExperienceProgressBar");
        _rect = GetNode<ColorRect>("VBoxContainer/HBoxContainer/TeamColorRect");
        _actionContainer = GetNode<Control>("VBoxContainer/HBoxContainer/VBoxContainer/HBoxContainer");
    }

    public override void _Process(float delta)
    {
        var camera = GetViewport().GetCamera3d();
        
        _healthBar.MaxValue = MaxHealth;
        _healthBar.Value = Health;

        _xpBar.MaxValue = MaxExperience;
        _xpBar.Value = Experience;

        _rect.Color = TeamColor;

        if (camera.IsPositionBehind(Position))
        {
            Hide();
        }
        else
        {
            Show();
            RectPosition = camera.UnprojectPosition(Position);
        }

        foreach(Node child in _actionContainer.GetChildren())
        {
            _actionContainer.RemoveChild(child);
            child.QueueFree();
        }

        for (int i = 0; i < MaxActions; i++)
        {
            var colorRect = new ColorRect();
            colorRect.SizeFlagsHorizontal = (int) Control.SizeFlags.ExpandFill;

            if (i < Actions)
            {
                colorRect.Color = new Color(0.8f, 1f, 0.8f);
            }
            else
            {
                colorRect.Color = new Color(0f, 0.4f, 0f);
            }
            _actionContainer.AddChild(colorRect);
        }
    }
}
