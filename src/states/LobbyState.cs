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

    private ISession _session;
    private ISocket _socket;
    private Client _client;
    private IChannel _channel;

    private IMatch _match;
    private IMatchmakerTicket _ticket;

    string _selectedMatchId = "";

    private List<IUserPresence> _users = new List<IUserPresence>();

    private string _mapName;
    private int _playerCount = 0;

    public LobbyState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        _view = Scenes.Instantiate<LobbyView>();

        _view.Connect(nameof(LobbyView.MatchSelected), new Callable(this, nameof(OnMatchSelected)));
        _view.Connect(nameof(LobbyView.MessageSubmitted), new Callable(this, nameof(OnMessageSubmitted)));
        _view.Connect(nameof(LobbyView.RefreshButtonPressed), new Callable(this, nameof(OnRefreshButtonPressed)));
        _view.Connect(nameof(LobbyView.JoinButtonPressed), new Callable(this, nameof(OnJoinButtonPressed)));
        _view.Connect(nameof(LobbyView.BackButtonPressed), new Callable(this, nameof(OnBackButtonPressed)));
        _view.Connect(nameof(LobbyView.ScenarioSelected), new Callable(this, nameof(OnScenarioSelected)));
        _view.Connect(nameof(LobbyView.CreateButtonPressed), new Callable(this, nameof(OnCreateButtonPressed)));
        _view.Connect(nameof(LobbyView.QueueButtonPressed), new Callable(this, nameof(OnQueueButtonPressed)));
        _view.Connect(nameof(LobbyView.CancelButtonPressed), new Callable(this, nameof(OnCancelButtonPressed)));

        AddChild(_view);

        _session = _world.GetResource<ISession>();
        _socket = _world.GetResource<ISocket>();
        _socket.ReceivedChannelMessage += OnReceivedChannelMessage;
        _socket.ReceivedChannelPresence += OnReceivedChannelPresence;
        _socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;

        _client = _world.GetResource<Client>();

        var account = _world.GetResource<IApiAccount>();
        _username = account.User.Username;

        var settings = _world.GetResource<LobbySettings>();
        EnterChat(settings.RoomName, ChannelType.Room, settings.Persistence, settings.Hidden);

        ListMatches();
    }

    private async void ListMatches()
    {
        var matches = await _client.ListMatchesAsync(_session, 0, 9, 20, false, null, null);
        _view.UpdateMatchList(matches);
    }

    public override void Continue(GameStateController gameStates)
    {
        _view.Show();
        _view.EnableJoinButton();
        _view.UpdateInfoLabel("");
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
        _socket.CloseAsync();
        _world.RemoveResource<ISocket>();
        _world.RemoveResource<ISession>();
        _world.RemoveResource<IApiAccount>();

        _view.QueueFree();
    }

    private async void EnterChat(string roomname, ChannelType channelType, bool persistence, bool hidden)
    {
        _channel = await _socket.JoinChatAsync(roomname, ChannelType.Room, persistence, hidden);
        _users.AddRange(_channel.Presences);
        _view.UpdateUsers(_username, _users);
    }

    private async void CreateMatch()
    {
        _match = await _socket.CreateMatchAsync();
        _world.AddResource(_match);
        _world.GetResource<GameStateController>().PushState(new FactionSelectionState(_world, _mapName));
    }

    private async void CreateMatchWith(IMatchmakerMatched matched)
    {
        _match = await _socket.JoinMatchAsync(matched);
        _world.AddResource(_match);
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
        _view.UpdateInfoLabel("Looking for Match...");
    }

    private void OnMatchSelected(string matchId)
    {
        _selectedMatchId = matchId;
    }

    private async void OnJoinButtonPressed()
    {
        _match = await _socket.JoinMatchAsync(_selectedMatchId);
        _world.AddResource(_match);
        _world.GetResource<GameStateController>().PushState(new FactionSelectionState(_world, _mapName));
    }

    private void OnRefreshButtonPressed()
    {
        ListMatches();
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

    private void OnCreateButtonPressed()
    {
        CreateMatch();
    }

    private void OnQueueButtonPressed()
    {
        if (string.IsNullOrEmpty(_mapName))
        {
            return;
        }

        _view.DisableQueueButton();
        JoinMatchmaking();
    }

    private void OnCancelButtonPressed()
    {
        if (_ticket != null)
        {
            _socket.RemoveMatchmakerAsync(_ticket);
            _ticket = null;
            _view.UpdateInfoLabel("Left Matchmaking");
        }

        if (_match != null)
        {
            _socket.LeaveMatchAsync(_match);
            _match = null;
            _view.UpdateInfoLabel("Closed Match");
        }

        _view.EnableJoinButton();
    }

    private void OnScenarioSelected(string mapName)
    {
        var mapData = Data.Instance.Maps[mapName];
        var playerList = mapData.Players;
        _mapName = mapName;
        _playerCount = playerList.Count;
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