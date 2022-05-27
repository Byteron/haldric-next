using System.Collections.Generic;
using RelEcs;
using Godot;
using Nakama;
using Nakama.TinyJson;

public partial class AttackSelectionState : GameState
{
    public Entity AttackerLocEntity { get; set; }
    public Entity DefenderLocEntity { get; set; }
    public Dictionary<Entity, Entity> AttackPairs { get; set; }
    public int AttackDistance { get; set; }

    public override void Init(GameStateController gameStates)
    {
        var initSystem = new AttackSelectionStateInitSystem();
        initSystem.AttackerLocEntity = AttackerLocEntity;
        initSystem.DefenderLocEntity = DefenderLocEntity;
        initSystem.AttackPairs = AttackPairs;
        initSystem.AttackDistance = AttackDistance;

        InitSystems.Add(initSystem);

        ExitSystems.Add(new AttackSelectionStateExitSystem());
    }
}

public partial class AttackSelectionStateInitSystem : Resource, ISystem
{
    public Entity AttackerLocEntity { get; set; }
    public Entity DefenderLocEntity { get; set; }
    public Dictionary<Entity, Entity> AttackPairs { get; set; }
    public int AttackDistance { get; set; }

    Commands _commands;

    public void Run(Commands commands)
    {
        _commands = commands;

        var canvas = commands.GetElement<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(5);

        var view = Scenes.Instantiate<AttackSelectionView>();
        view.Connect(nameof(AttackSelectionView.AttackSelected), new Callable(this, nameof(OnAttackSelected)));
        view.Connect(nameof(AttackSelectionView.CancelButtonPressed), new Callable(this, nameof(OnCancelButtonPressed)));
        canvasLayer.AddChild(view);

        view.UpdateInfo(AttackerLocEntity, DefenderLocEntity, AttackPairs);

        commands.AddElement(view);
    }

    void OnAttackSelected()
    {
        _commands.Send(new UnitDeselectedEvent());

        var commander = _commands.GetElement<Commander>();
        var view = _commands.GetElement<AttackSelectionView>();

        var attackerAttackEntity = view.GetSelectedAttackerAttack();
        var defenderAttackEntity = view.GetSelectedDefenderAttack();

        var seed = (ulong)(GD.Randi() % 9999);

        commander.Enqueue(new CombatCommand(seed, AttackerLocEntity, attackerAttackEntity, DefenderLocEntity, defenderAttackEntity, AttackDistance));

        var gameStateController = _commands.GetElement<GameStateController>();
        gameStateController.ChangeState(new CommanderState());

        if (!_commands.TryGetElement<ISocket>(out var socket)) return;
        if (!_commands.TryGetElement<IMatch>(out var match)) return;

        var message = new AttackUnitMessage
        {
            Seed = seed,
            Distance = AttackDistance,
            From = AttackerLocEntity.Get<Coords>(),
            To = DefenderLocEntity.Get<Coords>(),
            AttackerAttackId = attackerAttackEntity.Get<Id>().Value,
            DefenderAttackId = defenderAttackEntity.IsAlive ? defenderAttackEntity.Get<Id>().Value : "",
        };

        socket.SendMatchStateAsync(match.Id, (int)NetworkOperation.AttackUnit, message.ToJson());
    }

    void OnCancelButtonPressed()
    {
        var gameStateController = _commands.GetElement<GameStateController>();
        gameStateController.PopState();
    }
}

public class AttackSelectionStateExitSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.GetElement<AttackSelectionView>().QueueFree();
        commands.RemoveElement<AttackSelectionView>();
    }
}