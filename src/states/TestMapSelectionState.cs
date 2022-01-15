using Bitron.Ecs;
using Godot;

public partial class TestMapSelectionState : GameState
{
    private ScenarioSelectionView _view;

    public TestMapSelectionState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        _view = Scenes.Instantiate<ScenarioSelectionView>();

        _view.Connect("ContinuePressed", new Callable(this, nameof(OnContinuePressed)));
        _view.Connect("CancelPressed", new Callable(this, nameof(OnCancelPressed)));

        AddChild(_view);
    }

    public void OnContinuePressed(string mapName)
    {
        var gameStateController = _world.GetResource<GameStateController>();
        gameStateController.ChangeState(new TestMapState(_world, mapName));
    }

    public void OnCancelPressed()
    {
        var gameStateController = _world.GetResource<GameStateController>();
        gameStateController.PopState();
    }

    public override void Exit(GameStateController gameStates)
    {
        _view.QueueFree();
    }
}