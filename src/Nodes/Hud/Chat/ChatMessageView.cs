using Godot;

public partial class ChatMessageView : HBoxContainer
{
    public string User { get; set; }
    public string Message { get; set; }
    public string Time { get; set; }

     Label _messageLabel;
     Label _timeLabel;

    public override void _Ready()
    {
        _messageLabel = GetNode<Label>("MessageLabel");
        _timeLabel = GetNode<Label>("TimeLabel");

        _messageLabel.Text = $"{User}: {Message}";
        _timeLabel.Text = $"- {Time}";
    }
}
