using Godot;
using System;

public partial class UnitPlate : Control
{
    public Vector3 Position;

    public int MaxHealth;
    public int  Health;

    public int MaxMoves;
    public int  Moves;

    public int MaxExperience;
    public int  Experience;

    public Color TeamColor;

    public bool IsLeader;
    public bool IsHero;

    private TextureProgressBar _healthBar;
    private TextureProgressBar _xpBar;
    private ColorRect _teamColorRect;
    private ColorRect _heroColorRect;
    private ColorRect _leaderColorRect;
    private Control _actionContainer;

    public override void _Ready()
    {
        _healthBar = GetNode<TextureProgressBar>("VBoxContainer/HealthProgressBar");
        _xpBar = GetNode<TextureProgressBar>("VBoxContainer/HBoxContainer/VBoxContainer/ExperienceProgressBar");
        _teamColorRect = GetNode<ColorRect>("VBoxContainer/HBoxContainer/TeamColorRect");
        _leaderColorRect = GetNode<ColorRect>("VBoxContainer/LeaderColorRect");
        _heroColorRect = GetNode<ColorRect>("VBoxContainer/HeroColorRect");
        _actionContainer = GetNode<Control>("VBoxContainer/HBoxContainer/VBoxContainer/HBoxContainer");
    }

    public override void _Process(float delta)
    {
        var camera = GetViewport().GetCamera3d();
        
        _healthBar.MaxValue = MaxHealth;
        _healthBar.Value = Health;

        _xpBar.MaxValue = MaxExperience;
        _xpBar.Value = Experience;

        _teamColorRect.Color = TeamColor;

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

        for (int i = 0; i < MaxMoves; i++)
        {
            var colorRect = new ColorRect();
            colorRect.SizeFlagsHorizontal = (int) Control.SizeFlags.ExpandFill;

            if (i < Moves)
            {
                colorRect.Color = new Color(0.8f, 1f, 0.8f);
            }
            else
            {
                colorRect.Color = new Color(0f, 0.4f, 0f);
            }
            _actionContainer.AddChild(colorRect);
        }

        _leaderColorRect.Hide();
        _heroColorRect.Hide();
        
        if (IsHero)
        {
            _heroColorRect.Show();
        }

        if (IsLeader)
        {
            _leaderColorRect.Show();
        }
    }
}
