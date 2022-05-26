using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;

public class SpawnPlayersEvent
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

public class SpawnPlayersEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((SpawnPlayersEvent spawnEvent) =>
        {
            var players = spawnEvent.Players;
            var golds = spawnEvent.Golds;
            var factions = spawnEvent.Factions;

            foreach (var (startPosSide, coords) in commands.Query<IsStartingPositionOfSide, Coords>())
            {
                var side = startPosSide.Value;

                var playerId = players[side];
                var gold = golds[side];

                commands.Send(new SpawnPlayerEvent(playerId, side, coords, factions[side], gold));
            }
        });
    }
}