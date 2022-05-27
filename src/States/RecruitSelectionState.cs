using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;
using Nakama;
using Nakama.TinyJson;

public partial class RecruitSelectionState : GameState
{
    public Entity FreeLocEntity;

    public override void Init(GameStateController gameStates)
    {
        var initSystem = new RecruitSelectionStateInitSystem();
        initSystem.FreeLocEntity = FreeLocEntity;

        InitSystems.Add(initSystem);
        ExitSystems.Add(new RecruitSelectionStateExitSystem());
    }
}

public class RecruitSelectionStateExitSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.GetElement<RecruitSelectionView>().Cleanup();
        commands.GetElement<RecruitSelectionView>().QueueFree();
        commands.RemoveElement<RecruitSelectionView>();
    }
}

public partial class RecruitSelectionStateInitSystem : Resource, ISystem
{
    public int Side;
    public Entity FreeLocEntity;

    Commands _commands;

    public void Run(Commands commands)
    {
        _commands = commands;

        var scenario = commands.GetElement<Scenario>();
        var sideEntity = scenario.GetCurrentSideEntity();

        Side = scenario.Side;

        var canvas = commands.GetElement<Canvas>();
        var canvasLayer = canvas.GetCanvasLayer(2);

        var view = Scenes.Instantiate<RecruitSelectionView>();
        view.Connect(nameof(RecruitSelectionView.RecruitSelected), new Callable(this, nameof(OnRecruitSelected)));
        view.Connect(nameof(RecruitSelectionView.CancelButtonPressed), new Callable(this, nameof(OnCancelButtonPressed)));
        canvasLayer.AddChild(view);

        view.UpdateInfo(FreeLocEntity, sideEntity, sideEntity.Get<Recruits>().List);
        commands.AddElement(view);
    }

    void OnRecruitSelected(string unitTypeId)
    {
        var unitType = Data.Instance.Units[unitTypeId].Instantiate<UnitType>();
        
        var recruitEvent = new RecruitUnitTrigger(Side, unitType, FreeLocEntity);
        _commands.Send(recruitEvent);

        var gameStateController = _commands.GetElement<GameStateController>();
        gameStateController.PopState();

        if (!_commands.TryGetElement<ISocket>(out var socket)) return;
        if (!_commands.TryGetElement<IMatch>(out var match)) return;

        var message = new RecruitUnitMessage { Side = Side, UnitTypeId = unitTypeId, Coords = FreeLocEntity.Get<Coords>() };
        socket.SendMatchStateAsync(match.Id, (int)NetworkOperation.RecruitUnit, message.ToJson());
    }

    void OnCancelButtonPressed()
    {
        var gameStateController = _commands.GetElement<GameStateController>();
        gameStateController.PopState();
    }
}
