using System.Collections.Generic;
using Bitron.Ecs;

public partial class ScenarioSelectionState : GameState
{
    private ScenarioSelectionView _view;


    public ScenarioSelectionState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {   
        _view = Scenes.Instance.ScenarioSelectionView.Instantiate<ScenarioSelectionView>();   
        AddChild(_view);
    }

    public override void Exit(GameStateController gameStates)
    {
        _view.QueueFree();
    }
}