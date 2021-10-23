using Bitron.Ecs;

public partial class CommanderState : GameState
{
    public CommanderState(EcsWorld world) : base(world)
    { 
        AddUpdateSystem(new UpdateStatsInfoSystem());
        AddUpdateSystem(new ExecuteCommandSystem());
        AddUpdateSystem(new UpdateCameraOperatorSystem());
        AddUpdateSystem(new UpdateUnitPlateSystem());

        AddEventSystem<SpawnFloatingLabelEvent>(new SpawnFloatingLabelEventSystem());
    }

    public override void Enter(GameStateController gameStates)
    {

    }

    public override void Continue(GameStateController gameStates)
    {

    }

    public override void Pause(GameStateController gameStates)
    {

    }

    public override void Exit(GameStateController gameStates)
    {

    }
}