using System.Collections.Generic;
using System.Linq;
using Bitron.Ecs;
using Godot;
using Godot.Collections;
using Nakama;

public partial class MatchState : GameState
{
    private MatchView _view = null;
    private string _mapName = "";
    private int _playerCount = 0;
    private int _playersReadied = 0;

    private IMatchmakerTicket _ticket = null;
    private ISocket _socket = null;
    private IMatch _match = null;

    public MatchState(EcsWorld world, string mapName) : base(world)
    {
        _mapName = mapName;
        var mapDict = Data.Instance.Maps[mapName];
        var playerDict = (Dictionary)mapDict["Players"];
        _playerCount = playerDict.Count;
    }

    public override void Enter(GameStateController gameStates)
    {
        _view = Scenes.Instance.MatchView.Instantiate<MatchView>();
        _view.MapName = _mapName;
        _view.PlayerCount = _playerCount;
        _view.Connect("JoinButtonPressed", new Godot.Callable(this, nameof(OnJoinButtonPressed)));
        _view.Connect("CancelButtonPressed", new Godot.Callable(this, nameof(OnCancelButtonPressed)));
        AddChild(_view);

        _socket = _world.GetResource<ISocket>();
        _socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;
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
        _socket.ReceivedMatchmakerMatched -= OnReceivedMatchmakerMatched;

        if (_world.HasResource<IMatch>())
        {
            _world.RemoveResource<IMatch>();
        }

        if (_world.HasResource<LocalPlayer>())
        {
            _world.RemoveResource<LocalPlayer>();
        }

        _view.QueueFree();
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

    void OnJoinButtonPressed()
    {
        JoinMatchmaking();
    }

    void OnCancelButtonPressed()
    {
        if (_match == null && _ticket == null)
        {
            _world.GetResource<GameStateController>().PopState();
            return;
        }

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
    }

    private void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        CreateMatchWith(matched);
    }
}