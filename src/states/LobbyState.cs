using System;
using System.Linq;
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

    private IMatch _match;
    private IMatchmakerTicket _ticket;

    private List<IUserPresence> _users = new List<IUserPresence>();

    private string _mapName;
    private int _playerCount = 0;

    public LobbyState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        _view = Scenes.Instance.LobbyView.Instantiate<LobbyView>();

        _view.Connect(nameof(LobbyView.MessageSubmitted), new Callable(this, nameof(OnMessageSubmitted)));
        _view.Connect(nameof(LobbyView.BackButtonPressed), new Callable(this, nameof(OnBackButtonPressed)));
        _view.Connect(nameof(LobbyView.ScenarioSelected), new Callable(this, nameof(OnScenarioSelected)));
        _view.Connect(nameof(LobbyView.JoinButtonPressed), new Callable(this, nameof(OnJoinButtonPressed)));
        _view.Connect(nameof(LobbyView.CancelButtonPressed), new Callable(this, nameof(OnCancelButtonPressed)));

        AddChild(_view);

        _socket = _world.GetResource<ISocket>();
        _socket.ReceivedChannelMessage += OnReceivedChannelMessage;
        _socket.ReceivedChannelPresence += OnReceivedChannelPresence;
        _socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;

        var account = _world.GetResource<IApiAccount>();
        _username = account.User.Username;

        var settings = _world.GetResource<LobbySettings>();
        EnterChat(settings.RoomName, ChannelType.Room, settings.Persistence, settings.Hidden);
    }

    public override void Continue(GameStateController gameStates)
    {
        _view.Show();
        _view.EnableJoinButton();
        _view.UpdateInfo("");
    }

    public override void Pause(GameStateController gameStates)
    {
        _view.Hide();
    }

    public override void Exit(GameStateController gameStates)
    {
        OnCancelButtonPressed();

        _socket.ReceivedChannelMessage -= OnReceivedChannelMessage;
        _socket.ReceivedChannelPresence -= OnReceivedChannelPresence;
        _socket.ReceivedMatchmakerMatched -= OnReceivedMatchmakerMatched;

        _socket.LeaveChatAsync(_channel);

        _view.QueueFree();
    }

    private async void EnterChat(string roomname, ChannelType channelType, bool persistence, bool hidden)
    {
        _channel = await _socket.JoinChatAsync(roomname, ChannelType.Room, persistence, hidden);
        _users.AddRange(_channel.Presences);
        _view.UpdateUsers(_username, _users);
    }

    private async void CreateMatchWith(IMatchmakerMatched matched)
    {
        _match = await _socket.JoinMatchAsync(matched);

        var localPlayer = new LocalPlayer();
        var matchPlayers = new MatchPlayers();
        matchPlayers.Array = new IUserPresence[matched.Users.ToList().Count];

        localPlayer.Presence = matched.Self.Presence;

        var users = new List<string>();

        users.Add(localPlayer.Presence.Username);

        GD.Print("User: ", localPlayer.Presence.Username);

        var playerString = "";
        var playerId = 0;
        foreach (var user in matched.Users)
        {
            matchPlayers.Array[playerId] = user.Presence;

            playerString += user.Presence.Username + " ";

            if (matched.Self.Presence.UserId == user.Presence.UserId)
            {
                localPlayer.Side = playerId;
            }

            playerId += 1;
        }

        GD.Print("Side: ", localPlayer.Side);
        GD.Print("Players: ", playerString);

        _world.AddResource(_match);
        _world.AddResource(localPlayer);
        _world.AddResource(matchPlayers);

        _world.GetResource<GameStateController>().PushState(new FactionSelectionState(_world, _mapName));
    }

    private async void JoinMatchmaking()
    {
        if (_ticket != null || _match != null)
        {
            return;
        }

        var query = "*";
        var minCount = _playerCount;
        var maxCount = _playerCount;

        _ticket = await _socket.AddMatchmakerAsync(query, minCount, maxCount);
        _view.UpdateInfo("Looking for Match...");
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

    private void OnJoinButtonPressed()
    {
        if (string.IsNullOrEmpty(_mapName))
        {
            return;
        }

        _view.DisableJoinButton();
        JoinMatchmaking();
    }

    private void OnCancelButtonPressed()
    {
        if (_ticket != null)
        {
            _socket.RemoveMatchmakerAsync(_ticket);
            _ticket = null;
            _view.UpdateInfo("Left Matchmaking");
        }

        if (_match != null)
        {
            _socket.LeaveMatchAsync(_match);
            _match = null;
            _view.UpdateInfo("Closed Match");
        }

        _view.EnableJoinButton();
    }

    private void OnScenarioSelected(string mapName)
    {
        var mapDict = Data.Instance.Maps[mapName];
        var playerDict = (Godot.Collections.Dictionary)mapDict["Players"];
        _mapName = mapName;
        _playerCount = playerDict.Count;
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

    private void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        CreateMatchWith(matched);
    }
}