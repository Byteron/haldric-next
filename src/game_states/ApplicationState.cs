using Bitron.Ecs;

public partial class ApplicationState : GameState
{
    EcsEntity _menuEntity;

    public ApplicationState(EcsWorld world) : base(world)
    { 
        AddUpdateSystem(new UpdateStatsInfoSystem(this));
    }

    public override void Enter(GameStateController gameStates)
    {
        var menuView = Scenes.Instance.MainMenuView.Instantiate<MainMenuView>();
        AddChild(menuView);

        _menuEntity = _world.Spawn().Add(new NodeHandle<MainMenuView>(menuView));
    }

    public override void Continue(GameStateController gameStates)
    {
        var menuView = _menuEntity.Get<NodeHandle<MainMenuView>>().Node;
        menuView.Show();
    }

    public override void Pause(GameStateController gameStates)
    {
        var menuView = _menuEntity.Get<NodeHandle<MainMenuView>>().Node;
        menuView.Hide();
    }

    public override void Exit(GameStateController gameStates)
    {
        _menuEntity.Despawn();
    }
}