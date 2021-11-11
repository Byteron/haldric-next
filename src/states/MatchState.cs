using Bitron.Ecs;

public partial class MatchState : GameState
{
    MatchView _view;

    public MatchState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        _view = Scenes.Instance.MatchView.Instantiate<MatchView>();
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