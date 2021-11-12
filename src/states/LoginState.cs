using Bitron.Ecs;
using Godot;
using Nakama;

public partial class LoginState : GameState
{
    LoginView _view;

    public LoginState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        AddEventSystem<LoginEvent>(new LoginEventSystem());

        _view = Scenes.Instance.LoginView.Instantiate<LoginView>();

        _view.Connect("LoginPressed", new Callable(this, nameof(OnLoginPressed)));
        _view.Connect("CancelPressed", new Callable(this, nameof(OnCancelPressed)));
        
        AddChild(_view);
    }

    public override void Continue(GameStateController gameStates)
    {
        _view.Show();
    }

    public override void Pause(GameStateController gameStates)
    {
        _view.Hide();
    }

    public override void Exit(GameStateController gameStates)
    {
        _view.QueueFree();
    }

    public void OnLoginPressed(string email, string password, string username)
    {
        if (!_world.HasResource<Client>())
        {
            var settings = _world.GetResource<ServerSettings>();
            var client = new Client(settings.Scheme, settings.Host, settings.Port, settings.ServerKey);
            _world.AddResource(client);
        }

        _world.Spawn().Add(new LoginEvent(email, password, username));
    }

    public void OnCancelPressed()
    {
        _world.GetResource<GameStateController>().PopState();
    }
}