using System.Collections.Generic;
using Bitron.Ecs;

public struct SpawnUnitsEvent { }

public class SpawnUnitsEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var keepQuery = world.Query<Castle>().End();
        var eventQuery = world.Query<SpawnUnitsEvent>().End();

        foreach (var e in eventQuery)
        {
            var scenario = world.GetResource<Scenario>();
            
            int playerId = 0;
            foreach (var locEntityId in keepQuery)
            {
                var playerEntity = world.Spawn()
                    .Add(new Team(playerId))
                    .Add(new Gold(100))
                    .Add(new Recruits(new List<string>() { "Cavalryman", "Bowman", "Spearman"}));
                
                scenario.Players.Add(playerEntity);

                var locEntity = world.Entity(locEntityId);
                ref var keep = ref locEntity.Get<Castle>();

                world.Spawn().Add(new SpawnUnitEvent(playerId, "Dragoon", locEntity.Get<Coords>(), true));

                playerId += 1;
            }
        }
    }
}