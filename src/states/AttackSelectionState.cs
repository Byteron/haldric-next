using Bitron.Ecs;

public partial class AttackSelectionState : GameState
{
    private AttackSelectionView _view;

    public AttackSelectionState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        _view = Scenes.Instance.AttackSelectionView.Instantiate<AttackSelectionView>();
        AddChild(_view);
    }

    public override void Exit(GameStateController gameStates)
    {
        _view.QueueFree();
    }
}