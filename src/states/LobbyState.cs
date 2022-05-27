using System;
using System.Linq;
using System.Collections.Generic;
using RelEcs;
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
    public override void Init(GameStateController gameStates)
    {
        InitSystems.Add(new LobbyStateInitSystem());
        ContinueSystems.Add(new LobbyStateContinueSystem());
        PauseSystems.Add(new LobbyStatePauseSystem());
        ExitSystems.Add(new LobbyStateExitSystem());
    }
}

public class LobbyStateContinueSystem : ISystem
{
    public void Run(Commands commands)
    {
        var view = commands.GetElement<LobbyView>();
        view.Show();
        view.EnableJoinButton();
        view.UpdateInfo("");
    }
}

public class LobbyStatePauseSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.GetElement<LobbyView>().Hide();
    }
}

public class LobbyStateExitSystem : ISystem
{
    public void Run(Commands commands)
    {
        var view = commands.GetElement<LobbyView>();
        var socket = commands.GetElement<ISocket>();
        var channel = commands.GetElement<IChannel>();

        if (commands.TryGetElement<IMatchmakerTicket>(out var ticket))
        {
            socket.RemoveMatchmakerAsync(ticket);
            commands.RemoveElement<IMatchmakerTicket>();
        }

        if (commands.TryGetElement<IMatch>(out var match))
        {
            socket.LeaveMatchAsync(match);
            commands.RemoveElement<IMatch>();
        }

        socket.LeaveChatAsync(channel);
        socket.CloseAsync();

        commands.RemoveElement<IMatchmakerTicket>();
        commands.RemoveElement<IChannel>();
        commands.RemoveElement<ISocket>();
        commands.RemoveElement<ISession>();
        commands.RemoveElement<IApiAccount>();

        view.QueueFree();
        commands.RemoveElement<LobbyView>();
    }
}

public partial class LobbyStateInitSystem : Resource, ISystem
{
    LobbyView _view;

    string _username;

    ISocket _socket;
    IChannel _channel;

    IMatch _match;
    IMatchmakerTicket _ticket;

    List<IUserPresence> _users = new();

    string _mapName;
    int _playerCount;

    Commands _commands;

    public void Run(Commands commands)
    {
        _commands = commands;

        _view = Scenes.Instantiate<LobbyView>();

        _view.Connect(nameof(LobbyView.MessageSubmitted), new Callable(this, nameof(OnMessageSubmitted)));
        _view.Connect(nameof(LobbyView.BackButtonPressed), new Callable(this, nameof(OnBackButtonPressed)));
        _view.Connect(nameof(LobbyView.ScenarioSelected), new Callable(this, nameof(OnScenarioSelected)));
        _view.Connect(nameof(LobbyView.JoinButtonPressed), new Callable(this, nameof(OnJoinButtonPressed)));
        _view.Connect(nameof(LobbyView.CancelButtonPressed), new Callable(this, nameof(OnCancelButtonPressed)));

        commands.GetElement<CurrentGameState>().State.AddChild(_view);
        commands.AddElement(_view);

        _socket = commands.GetElement<ISocket>();
        _socket.ReceivedChannelMessage += OnReceivedChannelMessage;
        _socket.ReceivedChannelPresence += OnReceivedChannelPresence;
        _socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;

        var account = commands.GetElement<IApiAccount>();
        _username = account.User.Username;

        var settings = commands.GetElement<LobbySettings>();
        EnterChat(settings.RoomName, settings.Persistence, settings.Hidden);
    }

    async void EnterChat(string roomName, bool persistence, bool hidden)
    {
        _channel = await _socket.JoinChatAsync(roomName, ChannelType.Room, persistence, hidden);
        _commands.AddElement(_channel);
        _users.AddRange(_channel.Presences);
        _view.UpdateUsers(_username, _users);
    }

    async void CreateMatchWith(IMatchmakerMatched matched)
    {
        _match = await _socket.JoinMatchAsync(matched);

        var localPlayer = new LocalPlayer();
        var matchPlayers = new MatchPlayers
        {
            Array = new IUserPresence[matched.Users.ToList().Count]
        };

        localPlayer.Presence = matched.Self.Presence;

        GD.Print("User: ", localPlayer.Presence.Username);

        var playerString = "";
        var playerId = 0;
        foreach (var user in matched.Users)
        {
            matchPlayers.Array[playerId] = user.Presence;

            playerString += user.Presence.Username + " ";

            if (matched.Self.Presence.UserId == user.Presence.UserId)
            {
                localPlayer.Id = playerId;
            }

            playerId += 1;
        }

        GD.Print("Side: ", localPlayer.Id);
        GD.Print("Players: ", playerString);

        _commands.AddElement(_match);
        _commands.AddElement(localPlayer);
        _commands.AddElement(matchPlayers);

        _commands.GetElement<GameStateController>().PushState(new FactionSelectionState(_mapName));
    }

    async void JoinMatchmaking()
    {
        if (_ticket != null || _match != null) return;

        var query = "*";
        var minCount = _playerCount;
        var maxCount = _playerCount;

        _ticket = await _socket.AddMatchmakerAsync(query, minCount, maxCount);
        _commands.AddElement(_ticket);

        _view.UpdateInfo("Looking for Match...");
    }

    async void OnMessageSubmitted(string message)
    {
        var content = new ChannelMessage { Message = message }.ToJson();
        await _socket.WriteChatMessageAsync(_channel.Id, content);
    }

    void OnBackButtonPressed()
    {
        _commands.GetElement<GameStateController>().PopState();
    }

    void OnJoinButtonPressed()
    {
        if (string.IsNullOrEmpty(_mapName))
        {
            return;
        }

        _view.DisableJoinButton();
        JoinMatchmaking();
    }

    void OnCancelButtonPressed()
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

    void OnScenarioSelected(string mapName)
    {
        var mapData = Data.Instance.Maps[mapName];
        var playerList = mapData.Players;
        _mapName = mapName;
        _playerCount = playerList.Count;
    }

    void OnReceivedChannelPresence(IChannelPresenceEvent presenceEvent)
    {
        foreach (var presence in presenceEvent.Leaves)
        {
            _users.Remove(presence);
        }

        _users.AddRange(presenceEvent.Joins);
        _view.UpdateUsers(_username, _users);
    }

    void OnReceivedChannelMessage(IApiChannelMessage message)
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

    void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        CreateMatchWith(matched);
    }
}