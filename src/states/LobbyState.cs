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
    LobbyView view;

    string username;

    ISocket socket;
    IChannel channel;

    IMatch match;
    IMatchmakerTicket ticket;

    List<IUserPresence> users = new List<IUserPresence>();

    string mapName;
    int playerCount = 0;

    Commands commands;

    public void Run(Commands commands)
    {
        this.commands = commands;

        view = Scenes.Instantiate<LobbyView>();

        view.Connect(nameof(LobbyView.MessageSubmitted), new Callable(this, nameof(OnMessageSubmitted)));
        view.Connect(nameof(LobbyView.BackButtonPressed), new Callable(this, nameof(OnBackButtonPressed)));
        view.Connect(nameof(LobbyView.ScenarioSelected), new Callable(this, nameof(OnScenarioSelected)));
        view.Connect(nameof(LobbyView.JoinButtonPressed), new Callable(this, nameof(OnJoinButtonPressed)));
        view.Connect(nameof(LobbyView.CancelButtonPressed), new Callable(this, nameof(OnCancelButtonPressed)));

        commands.GetElement<CurrentGameState>().State.AddChild(view);
        commands.AddElement(view);

        socket = commands.GetElement<ISocket>();
        socket.ReceivedChannelMessage += OnReceivedChannelMessage;
        socket.ReceivedChannelPresence += OnReceivedChannelPresence;
        socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;

        var account = commands.GetElement<IApiAccount>();
        username = account.User.Username;

        var settings = commands.GetElement<LobbySettings>();
        EnterChat(settings.RoomName, ChannelType.Room, settings.Persistence, settings.Hidden);
    }

    async void EnterChat(string roomname, ChannelType channelType, bool persistence, bool hidden)
    {
        channel = await socket.JoinChatAsync(roomname, ChannelType.Room, persistence, hidden);
        commands.AddElement(channel);
        users.AddRange(channel.Presences);
        view.UpdateUsers(username, users);
    }

    async void CreateMatchWith(IMatchmakerMatched matched)
    {
        match = await socket.JoinMatchAsync(matched);

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
                localPlayer.Id = playerId;
            }

            playerId += 1;
        }

        GD.Print("Side: ", localPlayer.Id);
        GD.Print("Players: ", playerString);

        commands.AddElement(match);
        commands.AddElement(localPlayer);
        commands.AddElement(matchPlayers);

        commands.GetElement<GameStateController>().PushState(new FactionSelectionState(mapName));
    }

    async void JoinMatchmaking()
    {
        if (ticket != null || match != null)
        {
            return;
        }

        var query = "*";
        var minCount = playerCount;
        var maxCount = playerCount;

        ticket = await socket.AddMatchmakerAsync(query, minCount, maxCount);
        commands.AddElement(ticket);

        view.UpdateInfo("Looking for Match...");
    }

    async void OnMessageSubmitted(string message)
    {
        var content = new ChannelMessage { Message = message }.ToJson();
        var sendAck = await socket.WriteChatMessageAsync(channel.Id, content);
    }

    void OnBackButtonPressed()
    {
        commands.GetElement<GameStateController>().PopState();
    }

    void OnJoinButtonPressed()
    {
        if (string.IsNullOrEmpty(mapName))
        {
            return;
        }

        view.DisableJoinButton();
        JoinMatchmaking();
    }

    void OnCancelButtonPressed()
    {
        if (ticket != null)
        {
            socket.RemoveMatchmakerAsync(ticket);
            ticket = null;
            view.UpdateInfo("Left Matchmaking");
        }

        if (match != null)
        {
            socket.LeaveMatchAsync(match);
            match = null;
            view.UpdateInfo("Closed Match");
        }

        view.EnableJoinButton();
    }

    void OnScenarioSelected(string mapName)
    {
        var mapData = Data.Instance.Maps[mapName];
        var playerList = mapData.Players;
        this.mapName = mapName;
        playerCount = playerList.Count;
    }

    void OnReceivedChannelPresence(IChannelPresenceEvent presenceEvent)
    {
        foreach (var presence in presenceEvent.Leaves)
        {
            users.Remove(presence);
        }

        users.AddRange(presenceEvent.Joins);
        view.UpdateUsers(username, users);
    }

    void OnReceivedChannelMessage(IApiChannelMessage message)
    {

        switch (message.Code)
        {
            case 0:
                var channelMessage = JsonParser.FromJson<ChannelMessage>(message.Content);
                view.NewMessage(message.Username, channelMessage.Message, message.CreateTime);
                break;
            default:
                view.NewMessage(message.Username, message.Content, message.CreateTime);
                break;
        }
    }

    void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        CreateMatchWith(matched);
    }
}