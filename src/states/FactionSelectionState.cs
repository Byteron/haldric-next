using Godot;
using Godot.Collections;
using Bitron.Ecs;
using Nakama;
using Nakama.TinyJson;

public partial class FactionSelectionState : GameState
{
    private FactionSelectionView _view = null;

    public int _playerCount = 0;
    public int _playersReadied = 0;

    private string _mapName = "";
    private Dictionary _mapDict = null;

    private ISocket _socket = null;
    private IMatch _match = null;

    public FactionSelectionState(EcsWorld world, string mapName) : base(world)
    {
        _mapName = mapName;
        _mapDict = Data.Instance.Maps[mapName];
    }

    public override void Enter(GameStateController gameStates)
    {
        _socket = _world.GetResource<ISocket>();
        _match = _world.GetResource<IMatch>();

        var localPlayer = _world.GetResource<LocalPlayer>();
        var matchPlayers = _world.GetResource<MatchPlayers>();

        _playerCount = matchPlayers.Array.Length;

        _socket.ReceivedMatchState += OnMatchStateReceived;

        _view = Scenes.Instance.FactionSelectionView.Instantiate<FactionSelectionView>();

        var playerDict = (Dictionary)_mapDict["Players"];
        _view.MapName = _mapName;
        _view.LocalPlayerSide = localPlayer.Side;
        _view.PlayerCount = playerDict.Count;

        _view.Connect("FactionSelected", new Callable(this, nameof(OnFactionSelected)));
        _view.Connect("ContinueButtonPressed", new Callable(this, nameof(OnContinueButtonPressed)));
        _view.Connect("BackButtonPressed", new Callable(this, nameof(OnBackButtonPressed)));

        AddChild(_view);
    }

    public override void Exit(GameStateController gameStates)
    {
        _socket.ReceivedMatchState -= OnMatchStateReceived;

        _view.QueueFree();
    }

    public void CheckAndContinue()
    {
        if (_playerCount == _playersReadied)
        {
            var gameStateController = _world.GetResource<GameStateController>();

            var playState = new PlayState(_world, _mapName, _view.GetFactions());

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
            case NetworkOperation.FactionSelected:
                {
                    var message = JsonParser.FromJson<FactionSelectedMessage>(data);
                    _view.Select(message.Side, message.Index);
                    break;
                }
            case NetworkOperation.PlayerReadied:
                {
                    _playersReadied += 1;
                    CheckAndContinue();
                    break;
                }
        }
    }

    private void OnFactionSelected(int side, int index)
    {
        var matchId = _match.Id;
        var opCode = (int)NetworkOperation.FactionSelected;

        var message = new FactionSelectedMessage()
        {
            Side = side,
            Index = index,
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
        CheckAndContinue();
    }

    private void OnBackButtonPressed()
    {
        _world.GetResource<GameStateController>().PopState();
    }
}