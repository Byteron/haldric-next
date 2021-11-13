using Godot;

public partial class MatchView : Control
{
    [Signal] public delegate void JoinButtonPressed();
    [Signal] public delegate void CancelButtonPressed();

    public string MapName { get; set; } = "";
    public int PlayerCount { get; set; } = 2;

    Label _infoLabel;
    Label _mapLabel;

    public override void _Ready()
    {
        _infoLabel = GetNode<Label>("PanelContainer/CenterContainer/VBoxContainer/InfoLabel");
        _mapLabel = GetNode<Label>("PanelContainer/CenterContainer/VBoxContainer/MapLabel");

        _mapLabel.Text = $"Map: {MapName}, Players: {PlayerCount}";
    }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("ui_cancel"))
        {
            OnCancelButtonPressed();
        }
    }

    public void UpdateInfo(string text)
    {
        _infoLabel.Text = text;
    }

    void OnJoinButtonPressed()
    {
        EmitSignal(nameof(JoinButtonPressed));
    }

    void OnCancelButtonPressed()
    {
        EmitSignal(nameof(CancelButtonPressed));
    }
}
