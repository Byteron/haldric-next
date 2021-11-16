using Godot.Collections;
using Bitron.Ecs;

public struct SpawnPlayersEvent
{
    public Dictionary<int, string> Factions;

    public SpawnPlayersEvent(Dictionary<int, string> factions)
    {
        Factions = factions;
    }
}

public class SpawnPlayersEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var startingLocQuery = world.Query<IsStartingPositionOfSide>().End();
        var eventQuery = world.Query<SpawnPlayersEvent>().End();

        foreach (var e in eventQuery)
        {
            var matchPlayers = world.GetResource<MatchPlayers>();
            ref var spawnEvent = ref world.Entity(e).Get<SpawnPlayersEvent>();

            foreach (var locEntityId in startingLocQuery)
            {
                var locEntity = world.Entity(locEntityId);

                ref var coords = ref locEntity.Get<Coords>();
                ref var startPosSide = ref locEntity.Get<IsStartingPositionOfSide>();
                
                var username = matchPlayers.Array[startPosSide.Value].Username;

                world.Spawn().Add(new SpawnPlayerEvent(startPosSide.Value, username, coords, spawnEvent.Factions[startPosSide.Value], 100));
            }
        }
    }
}