using Godot;
using Bitron.Ecs;

public partial class ApplicationState : GameState
{
    public ApplicationState(EcsWorld world) : base(world) { }

    public override void Enter(GameStateController gameStates)
    {
        gameStates.PushState(new LoadingState(_world));
    }
}