using Leopotam.Ecs;

public partial class MainMenuState : GameState
{
    EcsEntity _menuEntity;

    public MainMenuState(EcsWorld world) : base(world)
    { 
        AddUpdateSystem(new UpdateStatsInfoSystem(this));
    }

    public override void Enter(GameStateController gameStates)
    {

        _menuEntity = _world.NewEntity();

        var menuView = Scenes.Instance.MainMenuView.Instantiate<MainMenuView>();
        AddChild(menuView);

        _menuEntity.Replace(new NodeHandle<MainMenuView>(menuView));
    }

    public override void Exit(GameStateController gameStates)
    {
        _menuEntity.Destroy();
    }
}