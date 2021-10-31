using System.Collections.Generic;
using Bitron.Ecs;

public partial class RecruitSelectionState : GameState
{
    public EcsEntity FreeLocEntity { get; set; }

    private RecruitSelectionView _view;

    public RecruitSelectionState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        var scenario = _world.GetResource<Scenario>();
        var player = scenario.GetCurrentPlayerEntity();

        var hudView = _world.GetResource<HUDView>();
        
        _view = Scenes.Instance.RecruitSelectionView.Instantiate<RecruitSelectionView>();
        
        hudView.AddChild(_view);

        _view.UpdateInfo(FreeLocEntity, player, player.Get<Recruits>().List);
    }

    public override void Exit(GameStateController gameStates)
    {
        _view.Cleanup();
        _view.QueueFree();
    }
}