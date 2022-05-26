using System.Linq;
using RelEcs;
using RelEcs.Godot;
using Godot;

public class SpawnPlayerEvent
{
    public int PlayerId;
    public int Side;
    public Coords Coords;
    public string Faction;
    public int Gold;

    public SpawnPlayerEvent(int playerId, int side, Coords coords, string faction, int gold)
    {
        PlayerId = playerId;
        Side = side;
        Coords = coords;
        Faction = faction;
        Gold = gold;
    }
}

public class SpawnPlayerEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((SpawnPlayerEvent spawnEvent) =>
        {
            var scenario = commands.GetElement<Scenario>();

            var username = "Username";

            if (commands.TryGetElement<MatchPlayers>(out var matchPlayers))
            {
                username = matchPlayers.Array[spawnEvent.PlayerId].Username;
            }

            FactionData faction;

            if (spawnEvent.Faction == "Random")
            {
                var factionName = Data.Instance.Factions.Keys.ToArray()[GD.Randi() % Data.Instance.Factions.Count];
                faction = Data.Instance.Factions[factionName];
            }
            else
            {
                faction = Data.Instance.Factions[spawnEvent.Faction];
            }

            GD.Print($"Spawning Player -  Id: {spawnEvent.PlayerId} | Name: {username} | Side: {spawnEvent.Side}");

            var sideEntity = commands.Spawn()
                .Add(new PlayerId { Value = spawnEvent.PlayerId })
                .Add(new Name { Value = username })
                .Add(new Side { Value = spawnEvent.Side })
                .Add(new Gold { Value = spawnEvent.Gold })
                .Add(new Faction { Value = faction.Name })
                .Add(new Recruits(faction.Recruits));

            scenario.Sides.Add(spawnEvent.Side, sideEntity);

            commands.Send(new SpawnUnitEvent(spawnEvent.Side, faction.Leaders[0], spawnEvent.Coords, true));
        });
    }
}