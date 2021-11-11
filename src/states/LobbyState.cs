using Bitron.Ecs;
using Nakama;

public partial class LobbyState : GameState
{
    LobbyView _view;

    public LobbyState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        
        _view = Scenes.Instance.LobbyView.Instantiate<LobbyView>();
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