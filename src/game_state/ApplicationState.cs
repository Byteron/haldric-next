using Bitron.Ecs;

public partial class ApplicationState : GameState
{
    public ApplicationState(EcsWorld world) : base(world)
    { 
        AddUpdateSystem(new UpdateStatsInfoSystem(this));
    }

    public override void Enter(GameStateController gameStates)
    {
        var menuView = Scenes.Instance.MainMenuView.Instantiate<MainMenuView>();
        AddChild(menuView);

        _world.AddResource(new NodeHandle<MainMenuView>(menuView));
    }

    public override void Continue(GameStateController gameStates)
    {
        _world.GetResource<NodeHandle<MainMenuView>>().Node.Show();
    }

    public override void Pause(GameStateController gameStates)
    {
        _world.GetResource<NodeHandle<MainMenuView>>().Node.Hide();
    }

    public override void Exit(GameStateController gameStates)
    {
        _world.RemoveResource<NodeHandle<MainMenuView>>();
    }
}