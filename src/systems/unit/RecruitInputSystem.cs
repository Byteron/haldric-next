using Bitron.Ecs;
using Godot;

public class RecruitInputSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (Input.IsActionJustPressed("recruit"))
        {
            var scenario = world.GetResource<Scenario>();
            var player = scenario.GetCurrentPlayerEntity();
            var team = player.Get<Team>();

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

                if (castleUnitEntity.Get<Team>().Value != team.Value)
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
            
            ref var castle = ref castleLocEntity.Get<Castle>();

            if (!castle.TryGetFreeLoc(out var freeLocEntity))
            {
                return;
            }
            var gameStateController = world.GetResource<GameStateController>();

            var recruitState = new RecruitSelectionState(world);
            recruitState.FreeLocEntity = freeLocEntity;
            
            gameStateController.PushState(recruitState);
        }
    }
}