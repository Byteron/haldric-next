using System.Collections.Generic;
using Godot;
using Nakama;
using Nakama.TinyJson;
using Bitron.Ecs;

public partial class LobbyView : Control
{
    public string Username { get; set; }

    private VBoxContainer _userListContainer;

    private VBoxContainer _messages;

    private LineEdit _input;

    private IChannel _channel;

    private ISocket _socket;

    private List<IUserPresence> _users = new List<IUserPresence>();

    public override void _Ready()
    {
        _userListContainer = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/VBoxContainer2/Panel/UserList");
        _messages = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/VBoxContainer/Panel/Messages");
        _input = GetNode<LineEdit>("PanelContainer/HBoxContainer/VBoxContainer/HBoxContainer/LineEdit");

        _socket = Main.Instance.World.GetResource<ISocket>();
        
        var settings = Main.Instance.World.GetResource<LobbySettings>();
        EnterChat(settings.RoomName, ChannelType.Room, settings.Persistence, settings.Hidden);
    }

    public override void _ExitTree()
    {
        _socket.ReceivedChannelMessage -= OnReceivedChannelMessage;
        _socket.ReceivedChannelPresence -= OnReceivedChannelPresence;

        _socket.LeaveChatAsync(_channel);
    }

    public async void EnterChat(string roomname, ChannelType channelType, bool persistence, bool hidden)
    {
        _socket.ReceivedChannelMessage += OnReceivedChannelMessage;
        _socket.ReceivedChannelPresence += OnReceivedChannelPresence;

        _channel = await _socket.JoinChatAsync(roomname, ChannelType.Room, persistence, hidden);
        _users.AddRange(_channel.Presences);

        UpdateUsers();
    }

    private void OnSendButtonPressed()
    {
        if (!string.IsNullOrEmpty(_input.Text))
        {
            SendMessage();
        }
    }

    private void OnBackButtonPressed()
    {
        Main.Instance.World.GetResource<GameStateController>().PopState();
    }

    private async void SendMessage()
    {
        var content = new Dictionary<string, string> { { "message", _input.Text } }.ToJson();
        _input.Text = "";
        var sendAck = await _socket.WriteChatMessageAsync(_channel.Id, content);
    }

    private void UpdateUsers()
    {
        foreach (Node child in _userListContainer.GetChildren())
        {
            _userListContainer.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var user in _users)
        {
            var label = new Label();
            label.Text = user.Username;

            if (user.Username == Username)
            {
                label.Modulate = new Color("AAAAFF");
            }

            _userListContainer.AddChild(label);
        }
    }

    private void NewMessage(string username, string message)
    {
        var label = new Label();
        label.Text = username + ": " + message;
        _messages.AddChild(label);

        if (_messages.GetChildCount() > 20)
        {
            var child = _messages.GetChild(0);
            _messages.RemoveChild(child);
            child.QueueFree();
        }
    }

    private void OnLineEditTextSubmitted(string text)
    {
        SendMessage();
    }

    private void OnReceivedChannelPresence(IChannelPresenceEvent presenceEvent)
    {
        foreach (var presence in presenceEvent.Leaves)
        {
            _users.Remove(presence);
        }

        _users.AddRange(presenceEvent.Joins);
        UpdateUsers();
    }

    private void OnReceivedChannelMessage(IApiChannelMessage message)
    {
        switch (message.Code)
        {
            case 0:
                var dict = JsonParser.FromJson<Dictionary<string, string>>(message.Content);
                NewMessage(message.Username, dict["message"]);
                break;
            default:
                NewMessage(message.Username, message.Content);
                break;
        }
    }
}
