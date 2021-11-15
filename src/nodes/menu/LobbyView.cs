using System.Collections.Generic;
using Godot;
using Nakama;

public partial class LobbyView : Control
{
    [Signal] public delegate void MessageSubmitted(string message);
    [Signal] public delegate void BackButtonPressed();

    [Export] private PackedScene ChatMessageView;

    private VBoxContainer _userListContainer;
    private VBoxContainer _messages;
    private LineEdit _input;

    public override void _Ready()
    {
        _userListContainer = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/VBoxContainer2/Panel/UserList");
        _messages = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/VBoxContainer/Panel/MarginContainer/Messages");
        _input = GetNode<LineEdit>("PanelContainer/HBoxContainer/VBoxContainer/HBoxContainer/LineEdit");
    }

    public void UpdateUsers(string username, List<IUserPresence> users)
    {
        foreach (Node child in _userListContainer.GetChildren())
        {
            _userListContainer.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var user in users)
        {
            var label = new Label();
            label.Text = user.Username;

            if (user.Username == username)
            {
                label.Modulate = new Color("AAAAFF");
            }

            _userListContainer.AddChild(label);
        }
    }

    public void NewMessage(string username, string message, string time)
    {
        var messageView = ChatMessageView.Instantiate<ChatMessageView>();
        messageView.Message = message;
        messageView.User = username;
        messageView.Time = time;

        _messages.AddChild(messageView);

        if (_messages.GetChildCount() > 20)
        {
            var child = _messages.GetChild(0);
            _messages.RemoveChild(child);
            child.QueueFree();
        }
    }

    private void OnLineEditTextSubmitted(string text)
    {
        OnSendButtonPressed();
    }

    private void OnSendButtonPressed()
    {
        if (!string.IsNullOrEmpty(_input.Text))
        {
            EmitSignal(nameof(MessageSubmitted), _input.Text);
            _input.Text = "";
        }
    }

    private void OnBackButtonPressed()
    {
        EmitSignal(nameof(BackButtonPressed));
    }
}
