using System.Collections.Generic;
using Bitron.Ecs;

public partial class AttackSelectionState : GameState
{
    public EcsEntity AttackerLocEntity { get; set; }
    public EcsEntity DefenderLocEntity { get; set; }
    public Dictionary<EcsEntity, EcsEntity> AttackPairs { get; set; }
    public int AttackDistance { get; set; }

    private AttackSelectionView _view;


    public AttackSelectionState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        var hudView = _world.GetResource<HUDView>();
        
        _view = Scenes.Instance.AttackSelectionView.Instantiate<AttackSelectionView>();
        
        hudView.AddChild(_view);

        _view.UpdateInfo(AttackerLocEntity, DefenderLocEntity, AttackPairs, AttackDistance);
    }

    public override void Exit(GameStateController gameStates)
    {
        _view.QueueFree();
    }
}