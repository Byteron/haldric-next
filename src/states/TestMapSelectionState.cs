using RelEcs;
using Godot;

public partial class TestMapSelectionState : GameState
{
    public override void Init(GameStateController gameStates)
    {
        InitSystems.Add(new TestMapSelectionStateInitSystem());
        ExitSystems.Add(new TestMapSelectionStateExitSystem());
    }
}

public class TestMapSelectionStateExitSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.GetElement<ScenarioSelectionView>().QueueFree();
        commands.RemoveElement<ScenarioSelectionView>();
    }
}

public partial class TestMapSelectionStateInitSystem : Resource, ISystem
{
    Commands commands;

    public void Run(Commands commands)
    {
        this.commands = commands;

        var view = Scenes.Instantiate<ScenarioSelectionView>();

        view.Connect("ContinuePressed", new Callable(this, nameof(OnContinuePressed)));
        view.Connect("CancelPressed", new Callable(this, nameof(OnCancelPressed)));

        commands.GetElement<CurrentGameState>().State.AddChild(view);
        commands.AddElement(view);
    }

    public void OnContinuePressed(string mapName)
    {
        var gameStateController = commands.GetElement<GameStateController>();
        gameStateController.ChangeState(new TestMapState(mapName));
    }

    public void OnCancelPressed()
    {
        var gameStateController = commands.GetElement<GameStateController>();
        gameStateController.PopState();
    }
}