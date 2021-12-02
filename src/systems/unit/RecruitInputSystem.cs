using Bitron.Ecs;
using Godot;

public class RecruitInputSystem : IEcsSystem
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

            var castleQuery = world.Query<Castle>().End();

            bool canRecruit = false;
            EcsEntity castleLocEntity = default;

            foreach (var castleLocEntityId in castleQuery)
            {
                castleLocEntity = world.Entity(castleLocEntityId);

                if (!castleLocEntity.Has<HasUnit>())
                {
                    continue;
                }

                var castleUnitEntity = castleLocEntity.Get<HasUnit>().Entity;

                if (!castleUnitEntity.Has<IsLeader>())
                {
                    continue;
                }

                if (castleUnitEntity.Get<Side>().Value != side.Value)
                {
                    continue;
                }

                canRecruit = true;
                break;
            }

            if (!canRecruit)
            {
                return;
            }

            EcsEntity freeLocEntity;

            ref var castle = ref castleLocEntity.Get<Castle>();

            var hLocEntity = world.GetResource<HoveredLocation>().Entity;

            if (hLocEntity.IsAlive() && castle.IsLocFree(hLocEntity.Get<Coords>()))
            {
                freeLocEntity = hLocEntity;
            }
            else if (!castle.TryGetFreeLoc(out freeLocEntity))
            {
                return;
            }

            var gameStateController = world.GetResource<GameStateController>();

            var recruitState = new RecruitSelectionState(world, freeLocEntity);

            gameStateController.PushState(recruitState);
        }
    }
}