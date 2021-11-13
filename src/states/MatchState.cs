using Bitron.Ecs;
using Godot.Collections;
using Nakama;

public partial class MatchState : GameState
{
    private MatchView _view;
    private string _mapName;
    private int _playerCount;

    private IMatchmakerTicket _ticket;
    private ISocket _socket;
    private IMatch _match;

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

        localPlayer.Presence = matched.Self.Presence;

        var playerId = 0;
        foreach (var presence in matched.Users)
        {
            if (matched.Self.Presence.UserId == presence.Presence.UserId)
            {
                localPlayer.Side = playerId;
            }
            playerId += 1;
        }

        _world.AddResource(localPlayer);
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