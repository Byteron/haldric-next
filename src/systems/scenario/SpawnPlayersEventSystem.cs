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
        world.ForEach((ref SpawnPlayersEvent spawnEvent) =>
        {
            var players = spawnEvent.Players;
            var golds = spawnEvent.Golds;
            var factions = spawnEvent.Factions;

            var matchPlayers = world.GetResource<MatchPlayers>();

            world.ForEach((EcsEntity locEntity, ref Coords coords, ref IsStartingPositionOfSide startPosSide) =>
            {
                var side = startPosSide.Value;

                var username = matchPlayers.Array[side].Username;
                var playerId = players[side];
                var gold = golds[side];

                world.Spawn().Add(new SpawnPlayerEvent(playerId, side, coords, factions[side], gold));
            });
        });
    }
}