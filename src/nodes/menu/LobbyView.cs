using System.Collections.Generic;
using Godot;
using Nakama;
using Nakama.TinyJson;

public partial class LobbyView : Control
{
    private string roomname = "general";
    private bool persistence = true;
    private bool hidden = false;

    private VBoxContainer _userList;
    private Dictionary<string, Label> _userLabels;

    private VBoxContainer _messages;

    private LineEdit _input;

    private IChannel _channel;

    public override void _Ready()
    {
        _userList = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/UserList");
        _messages = GetNode<VBoxContainer>("PanelContainer/HBoxContainer/VBoxContainer/Messages");
        _input = GetNode<LineEdit>("PanelContainer/HBoxContainer/VBoxContainer/HBoxContainer/LineEdit");

        EnterChat(roomname, ChannelType.Room, persistence, hidden);

        NewUser(Network.Instance.Account.User.Username);
    }

    public async void EnterChat(string roomname, ChannelType channelType, bool persistence, bool hidden)
    {
        var socket = Network.Instance.Socket;

        _channel = await socket.JoinChatAsync(roomname, ChannelType.Room, persistence, hidden);
        GD.Print("Now connected to channel id: ", _channel.Id);
        socket.ReceivedChannelMessage += OnReceivedChannelMessage;
    }

    private void OnSendButtonPressed()
    {
        if (!string.IsNullOrEmpty(_input.Text))
        {
            SendMessage();
        }
    }

    private async void SendMessage()
    {
        var socket = Network.Instance.Socket;
        var content = new Dictionary<string, string> { { "message", _input.Text } }.ToJson();
        _input.Text = "";
        var sendAck = await socket.WriteChatMessageAsync(_channel.Id, content);
    }

    private void NewUser(string username)
    {
        var label = new Label();
        label.Text = username;
        _userList.AddChild(label);
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
