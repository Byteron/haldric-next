public partial class CommanderState : GameState
{
    public override void Init(GameStateController gameStates)
    {
        UpdateSystems.Add(new UpdateStatsInfoSystem())
            .Add(new ExecuteCommandSystem())
            .Add(new UpdateCameraOperatorSystem())
            .Add(new UpdateUnitPlateSystem())
            .Add(new UnitDeselectedEventSystem())
            .Add(new DamageTriggerSystem())
            .Add(new MissTriggerSystem())
            .Add(new DeathTriggerSystem())
            .Add(new SpawnFloatingLabelEventSystem());
    }
}