using System.Collections.Generic;
using Bitron.Ecs;

public struct SpawnPlayersEvent { }

public class SpawnPlayersEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var keepQuery = world.Query<Castle>().End();
        var eventQuery = world.Query<SpawnPlayersEvent>().End();

        foreach (var e in eventQuery)
        {   
            int playerId = 0;
            foreach (var locEntityId in keepQuery)
            {
                var locEntity = world.Entity(locEntityId);
                ref var coords = ref locEntity.Get<Coords>();

                world.Spawn().Add(new SpawnPlayerEvent(playerId, coords, "Loyalists", 100));

                playerId += 1;
            }
        }
    }
}