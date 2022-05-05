using System.Collections.Generic;
using Godot;
using Nakama;

public partial class LobbyView : Control
{
    [Signal] public delegate void MessageSubmitted(string message);
    [Signal] public delegate void ScenarioSelected(string mapName);
    [Signal] public delegate void JoinButtonPressed();
    [Signal] public delegate void BackButtonPressed();
    [Signal] public delegate void CancelButtonPressed();

    [Export]  PackedScene ChatMessageView;

     VBoxContainer _userListContainer;
     VBoxContainer _messages;
     LineEdit _input;

     OptionButton _scenarioOptions;
     Button _joinButton;
     Label _infoLabel;

    public override void _Ready()
    {
        _userListContainer = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/VBoxContainer2/Panel/VBoxContainer/UserList");
        _messages = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/VBoxContainer/Panel/MarginContainer/Messages");
        _input = GetNode<LineEdit>("PanelContainer/HBoxContainer/VBoxContainer/HBoxContainer/LineEdit");

        _joinButton = GetNode<Button>("PanelContainer/HBoxContainer/VBoxContainer2/HBoxContainer/JoinButton");
        _infoLabel = GetNode<Label>("PanelContainer/HBoxContainer/VBoxContainer2/Label");
        _scenarioOptions = GetNode<OptionButton>("PanelContainer/HBoxContainer/VBoxContainer2/MapOptionButton");

        foreach (var mapName in Data.Instance.Maps.Keys)
        {
            _scenarioOptions.AddItem(mapName);
        }

        _scenarioOptions.Select(0);
        OnMapOptionButtonItemSelected(0);
    }

    public void UpdateInfo(string text)
    {
        _infoLabel.Text = text;
    }

    public void DisableJoinButton()
    {
        _joinButton.Disabled = true;
    }

    public void EnableJoinButton()
    {
        _joinButton.Disabled = false;
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

     void OnJoinButtonPressed()
    {
        EmitSignal(nameof(JoinButtonPressed));
    }

     void OnCancelButtonPressed()
    {
        EmitSignal(nameof(CancelButtonPressed));
    }

     void OnMapOptionButtonItemSelected(int index)
    {
        EmitSignal(nameof(ScenarioSelected), _scenarioOptions.GetItemText(index));
    }

     void OnLineEditTextSubmitted(string text)
    {
        OnSendButtonPressed();
    }

     void OnSendButtonPressed()
    {
        if (!string.IsNullOrEmpty(_input.Text))
        {
            EmitSignal(nameof(MessageSubmitted), _input.Text);
            _input.Text = "";
        }
    }

     void OnBackButtonPressed()
    {
        EmitSignal(nameof(BackButtonPressed));
    }
}
