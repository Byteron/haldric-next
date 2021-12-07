using System.Collections.Generic;
using Bitron.Ecs;
using Godot;
using Nakama;
using Nakama.TinyJson;

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
        var canvas = _world.GetResource<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(5);

        _view = Scenes.Instance.AttackSelectionView.Instantiate<AttackSelectionView>();
        _view.Connect("AttackSelected", new Callable(this, nameof(OnAttackSelected)));
        _view.Connect("CancelButtonPressed", new Callable(this, nameof(OnCancelButtonPressed)));
        canvasLayer.AddChild(_view);

        _view.UpdateInfo(AttackerLocEntity, DefenderLocEntity, AttackPairs);
    }

    public override void Exit(GameStateController gameStates)
    {
        _view.QueueFree();
    }

    private void OnAttackSelected()
    {
        _world.Spawn().Add(new UnitDeselectedEvent());

        var commander = _world.GetResource<Commander>();

        var attackerAttackEntity = _view.GetSelectedAttackerAttack();
        var defenderAttackEntity = _view.GetSelectedDefenderAttack();

        var seed = (ulong)(GD.Randi() % 9999);

        commander.Enqueue(new CombatCommand(seed, AttackerLocEntity, attackerAttackEntity, DefenderLocEntity, defenderAttackEntity, AttackDistance));

        var gameStateController = _world.GetResource<GameStateController>();
        gameStateController.ChangeState(new CommanderState(_world));

        if (!_world.TryGetResource<ISocket>(out var socket))
        {
            return;
        }

        if (!_world.TryGetResource<IMatch>(out var match))
        {
            return;
        }

        var message = new AttackUnitMessage
        {
            Seed = seed,
            Distance = AttackDistance,
            From = AttackerLocEntity.Get<Coords>(),
            To = DefenderLocEntity.Get<Coords>(),
            AttackerAttackId = attackerAttackEntity.Get<Id>().Value,
            DefenderAttackId = defenderAttackEntity.IsAlive() ? defenderAttackEntity.Get<Id>().Value : "",
        };

        socket.SendMatchStateAsync(match.Id, (int)NetworkOperation.AttackUnit, message.ToJson());
    }

    private void OnCancelButtonPressed()
    {
        var gameStateController = _world.GetResource<GameStateController>();
        gameStateController.PopState();
    }
}