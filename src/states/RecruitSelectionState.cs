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
        var sideEntity = scenario.GetCurrentSideEntity();

        _side = scenario.Side;

        var canvas = _world.GetResource<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(2);

        _view = Scenes.Instance.RecruitSelectionView.Instantiate<RecruitSelectionView>();
        _view.Connect(nameof(RecruitSelectionView.RecruitSelected), new Callable(this, nameof(OnRecruitSelected)));
        _view.Connect(nameof(RecruitSelectionView.CancelButtonPressed), new Callable(this, nameof(OnCancelButtonPressed)));
        canvasLayer.AddChild(_view);

        _view.UpdateInfo(_freeLocEntity, sideEntity, sideEntity.Get<Recruits>().List);
    }

    public override void Exit(GameStateController gameStates)
    {
        _view.Cleanup();
        _view.QueueFree();
    }

    private void OnRecruitSelected(string unitTypeId)
    {
        var unitType = Data.Instance.Units[unitTypeId].Instantiate<UnitType>();

        var coords = _freeLocEntity.Get<Coords>();

        var recruitEvent = new RecruitUnitEvent(_side, unitType, _freeLocEntity);
        _world.Spawn().Add(recruitEvent);

        var gameStateController = _world.GetResource<GameStateController>();
        gameStateController.PopState();

        if (!_world.TryGetResource<ISocket>(out var socket))
        {
            return;
        }

        if (!_world.TryGetResource<IMatch>(out var match))
        {
            return;
        }
        
        var message = new RecruitUnitMessage { Side = _side, UnitTypeId = unitTypeId, Coords = coords };
        socket.SendMatchStateAsync(match.Id, (int)NetworkOperation.RecruitUnit, message.ToJson());
    }

    private void OnCancelButtonPressed()
    {
        var gameStateController = _world.GetResource<GameStateController>();
        gameStateController.PopState();
    }
}