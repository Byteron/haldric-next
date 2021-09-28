using Bitron.Ecs;

public partial class ApplicationState : GameState
{
    EcsPackedEntity _menuEntityPacked;

    public ApplicationState(EcsWorld world) : base(world)
    { 
        AddUpdateSystem(new UpdateStatsInfoSystem(this));
    }

    public override void Enter(GameStateController gameStates)
    {
        var menuView = Scenes.Instance.MainMenuView.Instantiate<MainMenuView>();
        AddChild(menuView);
     
        int menuEntity = _world.Spawn().Add(new NodeHandle<MainMenuView>(menuView)).Entity();
        _menuEntityPacked = _world.PackEntity(menuEntity);
    }

    public override void Continue(GameStateController gameStates)
    {
        _menuEntityPacked.Unpack(_world, out var menuEntity);
        var menuView = _world.Entity(menuEntity).Get<NodeHandle<MainMenuView>>().Node;
        menuView.Show();
    }

    public override void Pause(GameStateController gameStates)
    {
        _menuEntityPacked.Unpack(_world, out var menuEntity);
        var menuView = _world.Entity(menuEntity).Get<NodeHandle<MainMenuView>>().Node;
        menuView.Hide();
    }

    public override void Exit(GameStateController gameStates)
    {
        _menuEntityPacked.Unpack(_world, out var menuEntity);
        _world.DespawnEntity(menuEntity);
    }
}