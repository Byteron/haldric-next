using System.Linq;
using Bitron.Ecs;
using Godot;

public struct SpawnPlayerEvent
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

public class SpawnPlayerEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<SpawnPlayerEvent>().End();

        foreach (var e in eventQuery)
        {
            ref var spawnEvent = ref world.Entity(e).Get<SpawnPlayerEvent>();

            var scenario = world.GetResource<Scenario>();

            var username = "Username";

            if (world.TryGetResource<MatchPlayers>(out var matchPlayers))
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

            var sideEntity = world.Spawn()
                .Add(new PlayerId(spawnEvent.PlayerId))
                .Add(new Name(username))
                .Add(new Side(spawnEvent.Side))
                .Add(new Gold(spawnEvent.Gold))
                .Add(new Faction(faction.Name))
                .Add(new Recruits(faction.Recruits));

            scenario.Sides.Add(spawnEvent.Side, sideEntity);

            world.Spawn().Add(new SpawnUnitEvent(spawnEvent.Side, faction.Leaders[0], spawnEvent.Coords, true));
        }
    }
}