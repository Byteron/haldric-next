using Godot;
using Godot.Collections;
using Bitron.Ecs;
using Nakama;
using Nakama.TinyJson;

public partial class FactionSelectionState : GameState
{
    private FactionSelectionView _view;

    private string _mapName;
    private Dictionary _mapDict;

    public FactionSelectionState(EcsWorld world, string mapName) : base(world)
    {
        _mapName = mapName;
        _mapDict = Data.Instance.Maps[mapName];
    }

    public override void Enter(GameStateController gameStates)
    {
        var socket = _world.GetResource<ISocket>();

        socket.ReceivedMatchState += OnMatchStateReceived;

        _view = Scenes.Instance.FactionSelectionView.Instantiate<FactionSelectionView>();

        var playerDict = (Dictionary)_mapDict["Players"];
        _view.MapName = _mapName;
        _view.PlayerCount = playerDict.Count;
        
        _view.Connect("FactionSelected", new Callable(this, nameof(OnFactionSelected)));
        _view.Connect("ContinueButtonPressed", new Callable(this, nameof(OnContinueButtonPressed)));
        _view.Connect("BackButtonPressed", new Callable(this, nameof(OnBackButtonPressed)));

        AddChild(_view);
    }

    public override void Exit(GameStateController gameStates)
    {
        var socket = _world.GetResource<ISocket>();
        socket.ReceivedMatchState -= OnMatchStateReceived;

        _view.QueueFree();
    }

    private void OnMatchStateReceived(IMatchState state)
    {
        var enc = System.Text.Encoding.UTF8;
        var data = (string)enc.GetString(state.State);
        var operation = (NetworkOperation)state.OpCode;

        switch (operation)
        {
            case NetworkOperation.FactionSelected:
                var message = JsonParser.FromJson<FactionSelectedMessage>(data);
                _view.Select(message.Side, message.Index);
                break;
        }
    }

    private void OnFactionSelected(int side, int index)
    {
        var matchId = _world.GetResource<IMatch>().Id;
        var opCode = (int)NetworkOperation.FactionSelected;

        var message = new FactionSelectedMessage()
        {
            Side = side,
            Index = index,
        };

        var socket = _world.GetResource<ISocket>();
        socket.SendMatchStateAsync(matchId, opCode, message.ToJson());
    }

    private void OnContinueButtonPressed(Dictionary<int, string> factions)
    {
        var gameStateController = _world.GetResource<GameStateController>();

        var playState = new PlayState(_world, _mapName, factions);
        
        gameStateController.ChangeState(playState);
    }

    private void OnBackButtonPressed()
    {
        _world.GetResource<GameStateController>().PopState();
    }
}