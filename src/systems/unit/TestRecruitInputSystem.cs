using Bitron.Ecs;
using Godot;

public class TestRecruitInputSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (Input.IsActionJustPressed("recruit"))
        {
            var scenario = world.GetResource<Scenario>();
            var localPlayer = world.GetResource<LocalPlayer>();
            var sideEntity = scenario.GetCurrentSideEntity();
            var side = sideEntity.Get<Side>();
            var playerId = sideEntity.Get<PlayerId>();

            if (localPlayer.Id != playerId.Value)
            {
                return;
            }

            var hLocEntity = world.GetResource<HoveredLocation>().Entity;

            if (!hLocEntity.IsAlive() && hLocEntity.Has<HasUnit>())
            {
                return;
            }

            var gameStateController = world.GetResource<GameStateController>();

            var recruitState = new RecruitSelectionState(world, hLocEntity);

            gameStateController.PushState(recruitState);
        }
    }
}