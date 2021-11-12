using Bitron.Ecs;
using Godot.Collections;

public partial class MatchState : GameState
{
    private MatchView _view;
    private string _mapName;
    private int _playerCount;

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
        AddChild(_view);
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
        _view.QueueFree();
    }
}