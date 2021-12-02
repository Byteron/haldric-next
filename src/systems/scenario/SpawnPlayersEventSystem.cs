using System.Collections.Generic;
using Bitron.Ecs;

public struct SpawnPlayersEvent
{
    public Dictionary<int, string> Factions;
    public Dictionary<int, int> Players;
    public Dictionary<int, int> Golds;

    public SpawnPlayersEvent(Dictionary<int, string> factions, Dictionary<int, int> players, Dictionary<int, int> golds)
    {
        Factions = factions;
        Players = players;
        Golds = golds;
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
                
                var side = startPosSide.Value;
                
                var username = matchPlayers.Array[side].Username;
                var playerId = spawnEvent.Players[side];
                var gold = spawnEvent.Golds[side];

                world.Spawn().Add(new SpawnPlayerEvent(playerId, side, coords, spawnEvent.Factions[side], gold));
            }
        }
    }
}