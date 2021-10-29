using System.Collections.Generic;
using Bitron.Ecs;

public class Faction
{
    public string Name;
    public List<string> Recruits;
    public List<string> Leaders;
}

public struct SpawnPlayerEvent
{
    public int Team;
    public Coords Coords;
    public string Faction;
    public int Gold;

    public SpawnPlayerEvent(int team, Coords coords, string faction, int gold)
    {
        Team = team;
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
            
            var faction = Data.Instance.Factions[spawnEvent.Faction];

            var playerEntity = world.Spawn()
                .Add(new Team(spawnEvent.Team))
                .Add(new Gold(spawnEvent.Gold))
                .Add(new Recruits(faction.Recruits));
            
            scenario.Players.Add(playerEntity);

            world.Spawn().Add(new SpawnUnitEvent(spawnEvent.Team, faction.Leaders[0], spawnEvent.Coords, true));
        }
    }
}