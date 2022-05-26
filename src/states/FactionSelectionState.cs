using Godot;
using RelEcs;
using Nakama;
using Nakama.TinyJson;
using System.Collections.Generic;

public partial class FactionSelectionState : GameState
{
    public int PlayerCount;
    public int PlayersReadied;
    
    string mapName;
    
    public FactionSelectionState(string mapName)
    {
        this.mapName = mapName;
    }

    public override void Init(GameStateController gameStates)
    {
        InitSystems.Add(new FactionSelectionStateInitSystem(mapName));
        ExitSystems.Add(new FactionSelectionStateExitSystem());
        UpdateSystems.Add(new FactionSelectionStateUpdateSystem(mapName));
    }
}

public class FactionSelectionStateUpdateSystem : ISystem
{
    readonly string mapName;

    public FactionSelectionStateUpdateSystem(string mapName)
    {
        this.mapName = mapName;
    }
    
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<CurrentGameState>(out var gameState)) return;
        if (gameState.State is not FactionSelectionState state) return;
        if (state.PlayerCount != state.PlayersReadied) return;
        if (!commands.TryGetElement<FactionSelectionView>(out var view)) return;
        
        var playState = new PlayState();
        playState.MapName = mapName;
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
    string mapName;

    FactionSelectionView view;

    ISocket socket;
    IMatch match;

    Commands commands;

    FactionSelectionState state;

    public FactionSelectionStateInitSystem(string mapName)
    {
        this.mapName = mapName;
    }
    
    public void Run(Commands commands)
    {
        this.commands = commands;

        socket = commands.GetElement<ISocket>();
        match = commands.GetElement<IMatch>();

        var localPlayer = commands.GetElement<LocalPlayer>();
        var matchPlayers = commands.GetElement<MatchPlayers>();
        
        state = commands.GetElement<CurrentGameState>().State as FactionSelectionState;

        state.PlayerCount = matchPlayers.Array.Length;

        socket.ReceivedMatchState += OnMatchStateReceived;

        view = Scenes.Instantiate<FactionSelectionView>();

        view.MapName = mapName;
        view.LocalPlayerId = localPlayer.Id;

        var players = new List<string>();

        foreach (var presence in matchPlayers.Array)
        {
            players.Add(presence.Username);
        }

        view.Players = players;

        view.Connect(nameof(FactionSelectionView.FactionChanged), new Callable(this, nameof(OnFactionChanged)));
        view.Connect(nameof(FactionSelectionView.PlayerChanged), new Callable(this, nameof(OnPlayerChanged)));
        view.Connect(nameof(FactionSelectionView.GoldChanged), new Callable(this, nameof(OnGoldChanged)));
        view.Connect(nameof(FactionSelectionView.ContinueButtonPressed), new Callable(this, nameof(OnContinueButtonPressed)));
        view.Connect(nameof(FactionSelectionView.BackButtonPressed), new Callable(this, nameof(OnBackButtonPressed)));

        commands.GetElement<CurrentGameState>().State.AddChild(view);
    }

    void OnMatchStateReceived(IMatchState state)
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
                    view.ChangeFaction(message.Side, message.Index);
                    break;
                }
            case NetworkOperation.PlayerChanged:
                {
                    var message = JsonParser.FromJson<PlayerChangedMessage>(data);
                    view.ChangePlayer(message.Side, message.Index);
                    break;
                }
            case NetworkOperation.GoldChanged:
                {
                    var message = JsonParser.FromJson<GoldChangedMessage>(data);
                    view.ChangeGold(message.Side, message.Value);
                    break;
                }
            case NetworkOperation.PlayerReadied:
                {
                    this.state.PlayersReadied += 1;
                    break;
                }
        }
    }

    void OnFactionChanged(int side, int index)
    {
        var matchId = match.Id;
        var opCode = (int)NetworkOperation.FactionChanged;

        var message = new FactionChangedMessage()
        {
            Side = side,
            Index = index,
        };

        socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    void OnPlayerChanged(int side, int index)
    {
        var matchId = match.Id;
        var opCode = (int)NetworkOperation.PlayerChanged;

        var message = new PlayerChangedMessage()
        {
            Side = side,
            Index = index,
        };

        socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    void OnGoldChanged(int side, int value)
    {
        var matchId = match.Id;
        var opCode = (int)NetworkOperation.GoldChanged;

        var message = new GoldChangedMessage()
        {
            Side = side,
            Value = value,
        };

        socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    void OnContinueButtonPressed()
    {
        state.PlayersReadied += 1;

        var matchId = match.Id;
        var opCode = (int)NetworkOperation.PlayerReadied;

        var message = new PlayerReadied();

        socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    void OnBackButtonPressed()
    {
        commands.RemoveElement<IMatch>();
        commands.RemoveElement<LocalPlayer>();
        commands.RemoveElement<MatchPlayers>();

        commands.GetElement<FactionSelectionView>().QueueFree();
        commands.RemoveElement<FactionSelectionView>();
        
        commands.GetElement<GameStateController>().PopState();
    }
}
