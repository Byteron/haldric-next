using Bitron.Ecs;

public partial class MenuState : GameState
{
    public MenuState(EcsWorld world) : base(world)
    { 
        AddUpdateSystem(new UpdateStatsInfoSystem());
    }

    public override void Enter(GameStateController gameStates)
    {
        var menuView = Scenes.Instance.MainMenuView.Instantiate<MainMenuView>();
        AddChild(menuView);

        var debugView = Scenes.Instance.DebugView.Instantiate<DebugView>();
        AddChild(debugView);

        _world.AddResource(menuView);
        _world.AddResource(debugView);
    }

    public override void Continue(GameStateController gameStates)
    {
        _world.GetResource<MainMenuView>().Show();
    }

    public override void Pause(GameStateController gameStates)
    {
        _world.GetResource<MainMenuView>().Hide();
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.RemoveResource<MainMenuView>();
        _world.RemoveResource<DebugView>();
    }
}