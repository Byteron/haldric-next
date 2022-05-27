using RelEcs;
using RelEcs.Godot;
using Godot;

public class TestRecruitInputSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (Input.IsActionJustPressed("recruit"))
        {
            var scenario = commands.GetElement<Scenario>();
            var localPlayer = commands.GetElement<LocalPlayer>();
            var sideEntity = scenario.GetCurrentSideEntity();
            var side = sideEntity.Get<Side>();
            var playerId = sideEntity.Get<PlayerId>();

            if (localPlayer.Id != playerId.Value) return;

            var hLocEntity = commands.GetElement<HoveredLocation>().Entity;

            if (!hLocEntity.IsAlive && hLocEntity.Has<HasUnit>()) return;

            var gameStateController = commands.GetElement<GameStateController>();

            var recruitState = new RecruitSelectionState();
            recruitState.FreeLocEntity = hLocEntity;
            gameStateController.PushState(recruitState);
        }
    }
}