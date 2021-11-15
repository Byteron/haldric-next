using Bitron.Ecs;
using Godot;
using Haldric.Wdk;
using Nakama;
using Nakama.TinyJson;

public partial class RecruitSelectionState : GameState
{
    private EcsEntity _freeLocEntity;

    private RecruitSelectionView _view;

    private int _side;

    public RecruitSelectionState(EcsWorld world, EcsEntity freeLocEntity) : base(world)
    {
        _freeLocEntity = freeLocEntity;
    }

    public override void Enter(GameStateController gameStates)
    {
        var scenario = _world.GetResource<Scenario>();
        var player = scenario.GetCurrentPlayerEntity();

        _side = scenario.CurrentPlayer;

        var hudView = _world.GetResource<HudView>();

        _view = Scenes.Instance.RecruitSelectionView.Instantiate<RecruitSelectionView>();
        _view.Connect("RecruitSelected", new Callable(this, nameof(OnRecruitSelected)));
        _view.Connect("CancelButtonPressed", new Callable(this, nameof(OnCancelButtonPressed)));
        hudView.AddChild(_view);


        _view.UpdateInfo(_freeLocEntity, player, player.Get<Recruits>().List);
    }

    public override void Exit(GameStateController gameStates)
    {
        _view.Cleanup();
        _view.QueueFree();
    }

    private void OnRecruitSelected(string unitTypeId)
    {
        var unitType = Data.Instance.Units[unitTypeId].Instantiate<UnitType>();

        var socket = _world.GetResource<ISocket>();
        var match = _world.GetResource<IMatch>();
        var coords = _freeLocEntity.Get<Coords>();
        var message = new RecruitUnitMessage { Side = _side, UnitTypeId = unitTypeId, Coords = coords };

        socket.SendMatchStateAsync(match.Id, (int)NetworkOperation.RecruitUnit, message.ToJson());


        var recruitEvent = new RecruitUnitEvent(_side, unitType, _freeLocEntity);
        _world.Spawn().Add(recruitEvent);

        var gameStateController = _world.GetResource<GameStateController>();
        gameStateController.PopState();
    }

    private void OnCancelButtonPressed()
    {
        var gameStateController = _world.GetResource<GameStateController>();
        gameStateController.PopState();
    }
}