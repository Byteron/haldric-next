using Godot;
using Bitron.Ecs;
using Nakama;
using Nakama.TinyJson;
using System.Collections.Generic;
using System.Linq;

public partial class FactionSelectionState : GameState
{
    private FactionSelectionView _view = null;

    public int _playerCount = 0;
    public int _playersReadied = 0;

    private string _mapName = "";
    private MapData _mapData = null;

    private ISocket _socket = null;
    private IMatch _match = null;

    private LocalPlayer _localPlayer = null;
    private MatchPlayers _matchPlayers = null;

    private Dictionary<string, int> _players = new Dictionary<string, int>();

    public FactionSelectionState(EcsWorld world, string mapName) : base(world)
    {
        _mapName = mapName;
        _mapData = Data.Instance.Maps[mapName];
    }

    public override void Enter(GameStateController gameStates)
    {
        _socket = _world.GetResource<ISocket>();
        _match = _world.GetResource<IMatch>();

        // GD.Print(_match.Self.Username);
        // GD.Print(_match.Presences.ToList().Count);

        // foreach (var presence in _match.Presences)
        // {
        //     GD.Print(presence.Username);
        // }

        _localPlayer = new LocalPlayer();
        _matchPlayers = new MatchPlayers();

        _localPlayer.Presence = _match.Self;
        _matchPlayers.Array = new IUserPresence[_mapData.Players.Count];
        _matchPlayers.Array[0] = _match.Self;

        // var playerString = "";
        // var playerId = 0;
        // foreach (var user in matched.Users)
        // {
        //     matchPlayers.Array[playerId] = user.Presence;

        //     playerString += user.Presence.Username + " ";

        //     if (matched.Self.Presence.UserId == user.Presence.UserId)
        //     {
        //         localPlayer.Id = playerId;
        //     }

        //     playerId += 1;
        // }

        _world.AddResource(_localPlayer);
        _world.AddResource(_matchPlayers);

        _players[_match.Self.Username] = 0;
        _playerCount = _matchPlayers.Array.Length;

        _socket.ReceivedMatchState += OnMatchStateReceived;
        _socket.ReceivedMatchPresence += OnReceivedMatchPresence;

        _view = Scenes.Instantiate<FactionSelectionView>();

        _view.MapName = _mapName;
        _view.LocalPlayerId = _localPlayer.Id;

        _view.PlayerCount = _playerCount;
        _view.Players = _players.Keys.ToList();

        _view.Connect(nameof(FactionSelectionView.FactionChanged), new Callable(this, nameof(OnFactionChanged)));
        _view.Connect(nameof(FactionSelectionView.PlayerChanged), new Callable(this, nameof(OnPlayerChanged)));
        _view.Connect(nameof(FactionSelectionView.GoldChanged), new Callable(this, nameof(OnGoldChanged)));
        _view.Connect(nameof(FactionSelectionView.ContinueButtonPressed), new Callable(this, nameof(OnContinueButtonPressed)));
        _view.Connect(nameof(FactionSelectionView.BackButtonPressed), new Callable(this, nameof(OnBackButtonPressed)));

        AddChild(_view);
    }

    public override void Exit(GameStateController gameStates)
    {
        _socket.ReceivedMatchState -= OnMatchStateReceived;
        _socket.ReceivedMatchPresence -= OnReceivedMatchPresence;
        _view.QueueFree();
    }

    public override void _Process(float delta)
    {
        CheckAndContinue();
    }

    public void CheckAndContinue()
    {
        if (_playerCount == _playersReadied)
        {
            var gameStateController = _world.GetResource<GameStateController>();

            var playState = new PlayState(_world, _mapName, _view.GetFactions(), _view.GetPlayers(), _view.GetPlayerGolds());

            gameStateController.ChangeState(playState);
        }
    }

    private void OnMatchStateReceived(IMatchState state)
    {
        var enc = System.Text.Encoding.UTF8;
        var data = (string)enc.GetString(state.State);
        var operation = (NetworkOperation)state.OpCode;

        GD.Print($"Network Operation: {operation.ToString()}\nJSON: {data}");

        switch (operation)
        {
            case NetworkOperation.FactionChanged:
                {
                    var message = JsonParser.FromJson<FactionChangedMessage>(data);
                    _view.ChangeFaction(message.Side, message.Index);
                    break;
                }
            case NetworkOperation.PlayerChanged:
                {
                    var message = JsonParser.FromJson<PlayerChangedMessage>(data);
                    _view.ChangePlayer(message.Side, message.Index);
                    break;
                }
            case NetworkOperation.GoldChanged:
                {
                    var message = JsonParser.FromJson<GoldChangedMessage>(data);
                    _view.ChangeGold(message.Side, message.Value);
                    break;
                }
            case NetworkOperation.PlayerReadied:
                {
                    _playersReadied += 1;
                    break;
                }
        }
    }

    private void OnReceivedMatchPresence(IMatchPresenceEvent presenceEvent)
    {
        foreach (var joined in presenceEvent.Joins)
        {
            _players.Add(joined.Username, _players.Count);
            GD.Print($"Player Joined: {joined.Username}");
        }

        foreach (var left in presenceEvent.Leaves)
        {
            _players.Remove(left.Username);
            GD.Print($"Player Left: {left.Username}");
        }

        var keys = _players.Keys.ToArray();
        _players.Clear();

        foreach (var key in keys)
        {
            _players.Add(key, _players.Count);
        }

        _view.UpdatePlayers(_players.Keys.ToList());
    }

    private void OnFactionChanged(int side, int index)
    {
        var matchId = _match.Id;
        var opCode = (int)NetworkOperation.FactionChanged;

        var message = new FactionChangedMessage()
        {
            Side = side,
            Index = index,
        };

        _socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    private void OnPlayerChanged(int side, int index)
    {
        var matchId = _match.Id;
        var opCode = (int)NetworkOperation.PlayerChanged;

        var message = new PlayerChangedMessage()
        {
            Side = side,
            Index = index,
        };

        _socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    private void OnGoldChanged(int side, int value)
    {
        var matchId = _match.Id;
        var opCode = (int)NetworkOperation.GoldChanged;

        var message = new GoldChangedMessage()
        {
            Side = side,
            Value = value,
        };

        _socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    private void OnContinueButtonPressed()
    {
        _playersReadied += 1;

        var matchId = _match.Id;
        var opCode = (int)NetworkOperation.PlayerReadied;

        var message = new PlayerReadied();

        _socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    private void OnBackButtonPressed()
    {
        _world.RemoveResource<IMatch>();
        _world.RemoveResource<LocalPlayer>();
        _world.RemoveResource<MatchPlayers>();

        _world.GetResource<GameStateController>().PopState();
    }
}