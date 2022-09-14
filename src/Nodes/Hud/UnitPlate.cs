using Godot;

public partial class UnitPlate : Control
{
    public Vector3 WorldPosition { get; set; }

    public int MaxHealth { get; set; }
    public int Health { get; set; }

    public int MaxMoves { get; set; }
    public int Moves { get; set; }

    public int MaxExperience { get; set; }
    public int Experience { get; set; }

    public Color SideColor { get; set; }

    public bool IsLeader;
    public bool IsHero;

     TextureProgressBar _healthBar;
     TextureProgressBar _xpBar;
     ColorRect _sideColorRect;
     ColorRect _heroColorRect;
     ColorRect _leaderColorRect;
     Control _actionContainer;

    public override void _Ready()
    {
        _healthBar = GetNode<TextureProgressBar>("VBoxContainer/HealthProgressBar");
        _xpBar = GetNode<TextureProgressBar>("VBoxContainer/HBoxContainer/VBoxContainer/ExperienceProgressBar");
        _sideColorRect = GetNode<ColorRect>("VBoxContainer/HBoxContainer/SideColorRect");
        _leaderColorRect = GetNode<ColorRect>("VBoxContainer/LeaderColorRect");
        _heroColorRect = GetNode<ColorRect>("VBoxContainer/HeroColorRect");
        _actionContainer = GetNode<Control>("VBoxContainer/HBoxContainer/VBoxContainer/HBoxContainer");
    }

    public override void _Process(double delta)
    {
        var camera = GetViewport().GetCamera3d();

        if (camera == null)
        {
            QueueFree();
            return;
        }

        _healthBar.MaxValue = MaxHealth;
        _healthBar.Value = Health;

        _xpBar.MaxValue = MaxExperience;
        _xpBar.Value = Experience;

        _sideColorRect.Color = SideColor;

        if (camera.IsPositionBehind(WorldPosition))
        {
            Hide();
        }
        else
        {
            Show();
            Position = camera.UnprojectPosition(WorldPosition);
        }

        foreach (Node child in _actionContainer.GetChildren())
        {
            _actionContainer.RemoveChild(child);
            child.QueueFree();
        }

        for (var i = 0; i < MaxMoves; i++)
        {
            var colorRect = new ColorRect
            {
                SizeFlagsHorizontal = (int)SizeFlags.ExpandFill
            };

            colorRect.Color = i < Moves ? new Color(0.8f, 1f, 0.8f) : new Color(0f, 0.4f, 0f);
            
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
