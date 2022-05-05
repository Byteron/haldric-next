using RelEcs;
using Godot;
using Nakama;

public partial class LoginState : GameState
{
    public override void Init(GameStateController gameStates)
    {
        InitSystems.Add(new LoginStateInitSystem());
        ContinueSystems.Add(new LoginStateContinueSystem());
        PauseSystems.Add(new LoginStatePauseSystem());
        ExitSystems.Add(new LoginStateExitSystem());

        UpdateSystems.Add(new LoginEventSystem());
    }  
}

public class LoginStateExitSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.GetElement<LoginView>().QueueFree();
        commands.RemoveElement<LoginView>();
    }
}

public class LoginStateContinueSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.GetElement<LoginView>().Show();
    }
}

public class LoginStatePauseSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.GetElement<LoginView>().Hide();
    }
}

public partial class LoginStateInitSystem : Resource, ISystem
{
    Commands commands;

    public void Run(Commands commands)
    {
        this.commands = commands;

        var view = Scenes.Instantiate<LoginView>();

        view.Connect("LoginPressed", new Callable(this, nameof(OnLoginPressed)));
        view.Connect("CancelPressed", new Callable(this, nameof(OnCancelPressed)));

        var currentState = commands.GetElement<CurrentGameState>();
        currentState.State.AddChild(view);

        commands.AddElement(view);
    }

    public void OnLoginPressed(string email, string password, string username)
    {
        if (!commands.HasElement<Client>())
        {
            var settings = commands.GetElement<ServerSettings>();
            var client = new Client(settings.Scheme, settings.Host, settings.Port, settings.ServerKey);
            commands.AddElement(client);
        }

        commands.Send(new LoginEvent(email, password, username));
    }

    public void OnCancelPressed()
    {
        commands.GetElement<GameStateController>().PopState();
    }
}