using System;
using System.Collections.Generic;
using Bitron.Ecs;
using Godot;
using Nakama;
using Nakama.TinyJson;

[Serializable]
public struct ChannelMessage
{
    public string Message;
}

public partial class LobbyState : GameState
{
    LobbyView _view;

    string _username;

    private ISocket _socket;
    private IChannel _channel;

    private List<IUserPresence> _users = new List<IUserPresence>();

    public LobbyState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        _view = Scenes.Instance.LobbyView.Instantiate<LobbyView>();
        _view.Connect("MessageSubmitted", new Callable(this, nameof(OnMessageSubmitted)));
        _view.Connect("BackButtonPressed", new Callable(this, nameof(OnBackButtonPressed)));
        AddChild(_view);

        _socket = _world.GetResource<ISocket>();
        _socket.ReceivedChannelMessage += OnReceivedChannelMessage;
        _socket.ReceivedChannelPresence += OnReceivedChannelPresence;

        var account = _world.GetResource<IApiAccount>();
        _username = account.User.Username;

        var settings = _world.GetResource<LobbySettings>();
        EnterChat(settings.RoomName, ChannelType.Room, settings.Persistence, settings.Hidden);
    }

    public override void Continue(GameStateController gameStates)
    {
        _view.Show();
    }

    public override void Pause(GameStateController gameStates)
    {
        _view.Hide();
    }

    public override void Exit(GameStateController gameStates)
    {
        _socket.ReceivedChannelMessage -= OnReceivedChannelMessage;
        _socket.ReceivedChannelPresence -= OnReceivedChannelPresence;

        _socket.LeaveChatAsync(_channel);

        _view.QueueFree();
    }

    private async void EnterChat(string roomname, ChannelType channelType, bool persistence, bool hidden)
    {
        _channel = await _socket.JoinChatAsync(roomname, ChannelType.Room, persistence, hidden);
        _users.AddRange(_channel.Presences);
        _view.UpdateUsers(_username, _users);
    }

    private async void OnMessageSubmitted(string message)
    {
        var content = new ChannelMessage { Message = message }.ToJson();
        var sendAck = await _socket.WriteChatMessageAsync(_channel.Id, content);
    }

    private void OnBackButtonPressed()
    {
        _world.GetResource<GameStateController>().PopState();
    }

    private void OnReceivedChannelPresence(IChannelPresenceEvent presenceEvent)
    {
        foreach (var presence in presenceEvent.Leaves)
        {
            _users.Remove(presence);
        }

        _users.AddRange(presenceEvent.Joins);
        _view.UpdateUsers(_username, _users);
    }

    private void OnReceivedChannelMessage(IApiChannelMessage message)
    {
        
        switch (message.Code)
        {
            case 0:
                var channelMessage = JsonParser.FromJson<ChannelMessage>(message.Content);
                _view.NewMessage(message.Username, channelMessage.Message, message.CreateTime);
                break;
            default:
                _view.NewMessage(message.Username, message.Content, message.CreateTime);
                break;
        }
    }
}