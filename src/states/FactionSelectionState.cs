using Godot;
using RelEcs;
using Nakama;
using Nakama.TinyJson;
using System.Collections.Generic;

public partial class FactionSelectionState : GameState
{
    public int PlayerCount;
    public int PlayersReadied;

    readonly string _mapName;
    
    public FactionSelectionState(string mapName) => _mapName = mapName;

    public override void Init(GameStateController gameStates)
    {
        InitSystems.Add(new FactionSelectionStateInitSystem(_mapName));
        ExitSystems.Add(new FactionSelectionStateExitSystem());
        UpdateSystems.Add(new FactionSelectionStateUpdateSystem(_mapName));
    }
}

public class FactionSelectionStateUpdateSystem : ISystem
{
    readonly string _mapName;

    public FactionSelectionStateUpdateSystem(string mapName) => _mapName = mapName;
    
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<CurrentGameState>(out var gameState)) return;
        if (gameState.State is not FactionSelectionState state) return;
        if (state.PlayerCount != state.PlayersReadied) return;
        if (!commands.TryGetElement<FactionSelectionView>(out var view)) return;
        
        var playState = new PlayState();
        playState.MapName = _mapName;
        playState.Factions = view.GetFactions();
        playState.Players = view.GetPlayers();
        playState.PlayerGolds = view.GetPlayerGolds();
        commands.GetElement<GameStateController>().ChangeState(playState);
    }
}
public class FactionSelectionStateExitSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.GetElement<FactionSelectionView>().QueueFree();
        commands.RemoveElement<FactionSelectionView>();
    }
}

public partial class FactionSelectionStateInitSystem : Resource, ISystem
{
    readonly string _mapName;

    FactionSelectionView _view;

    ISocket _socket;
    IMatch _match;

    Commands _commands;

    FactionSelectionState _state;

    public FactionSelectionStateInitSystem(string mapName) => _mapName = mapName;
    
    public void Run(Commands commands)
    {
        _commands = commands;

        _socket = commands.GetElement<ISocket>();
        _match = commands.GetElement<IMatch>();

        var localPlayer = commands.GetElement<LocalPlayer>();
        var matchPlayers = commands.GetElement<MatchPlayers>();
        
        _state = commands.GetElement<CurrentGameState>().State as FactionSelectionState;

        if (_state != null) _state.PlayerCount = matchPlayers.Array.Length;

        _socket.ReceivedMatchState += OnMatchStateReceived;

        _view = Scenes.Instantiate<FactionSelectionView>();

        _view.MapName = _mapName;
        _view.LocalPlayerId = localPlayer.Id;

        var players = new List<string>();

        foreach (var presence in matchPlayers.Array)
        {
            players.Add(presence.Username);
        }

        _view.Players = players;

        _view.Connect(nameof(FactionSelectionView.FactionChanged), new Callable(this, nameof(OnFactionChanged)));
        _view.Connect(nameof(FactionSelectionView.PlayerChanged), new Callable(this, nameof(OnPlayerChanged)));
        _view.Connect(nameof(FactionSelectionView.GoldChanged), new Callable(this, nameof(OnGoldChanged)));
        _view.Connect(nameof(FactionSelectionView.ContinueButtonPressed), new Callable(this, nameof(OnContinueButtonPressed)));
        _view.Connect(nameof(FactionSelectionView.BackButtonPressed), new Callable(this, nameof(OnBackButtonPressed)));
        
        commands.AddElement(_view);
        
        commands.GetElement<CurrentGameState>().State.AddChild(_view);
    }

    void OnMatchStateReceived(IMatchState state)
    {
        var enc = System.Text.Encoding.UTF8;
        var data = enc.GetString(state.State);
        var operation = (NetworkOperation)state.OpCode;

        GD.Print($"Network Operation: {operation.ToString()}\nJSON: {data}");

        switch (operation)
        {
            case NetworkOperation.FactionChanged:
                {
                    var message = data.FromJson<FactionChangedMessage>();
                    _view.ChangeFaction(message.Side, message.Index);
                    break;
                }
            case NetworkOperation.PlayerChanged:
                {
                    var message = data.FromJson<PlayerChangedMessage>();
                    _view.ChangePlayer(message.Side, message.Index);
                    break;
                }
            case NetworkOperation.GoldChanged:
                {
                    var message = data.FromJson<GoldChangedMessage>();
                    _view.ChangeGold(message.Side, message.Value);
                    break;
                }
            case NetworkOperation.PlayerReadied:
                {
                    GD.Print("PlayerReadied Received");
                    _state.PlayersReadied += 1;
                    break;
                }
        }
    }

    void OnFactionChanged(int side, int index)
    {
        var matchId = _match.Id;
        const int opCode = (int)NetworkOperation.FactionChanged;

        var message = new FactionChangedMessage()
        {
            Side = side,
            Index = index,
        };

        _socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    void OnPlayerChanged(int side, int index)
    {
        var matchId = _match.Id;
        const int opCode = (int)NetworkOperation.PlayerChanged;

        var message = new PlayerChangedMessage()
        {
            Side = side,
            Index = index,
        };

        _socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    void OnGoldChanged(int side, int value)
    {
        var matchId = _match.Id;
        const int opCode = (int)NetworkOperation.GoldChanged;

        var message = new GoldChangedMessage()
        {
            Side = side,
            Value = value,
        };

        _socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    void OnContinueButtonPressed()
    {
        _state.PlayersReadied += 1;

        var matchId = _match.Id;
        const int opCode = (int)NetworkOperation.PlayerReadied;

        var message = new PlayerReadied();

        _socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    void OnBackButtonPressed()
    {
        _commands.RemoveElement<IMatch>();
        _commands.RemoveElement<LocalPlayer>();
        _commands.RemoveElement<MatchPlayers>();

        _commands.GetElement<FactionSelectionView>().QueueFree();
        _commands.RemoveElement<FactionSelectionView>();
        
        _commands.GetElement<GameStateController>().PopState();
    }
}
