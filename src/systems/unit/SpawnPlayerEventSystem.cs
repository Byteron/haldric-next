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
    public int Side;
    public Coords Coords;
    public string Faction;
    public int Gold;

    public SpawnPlayerEvent(int side, Coords coords, string faction, int gold)
    {
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
            
            var faction = Data.Instance.Factions[spawnEvent.Faction];

            var playerEntity = world.Spawn()
                .Add(new Side(spawnEvent.Side))
                .Add(new Gold(spawnEvent.Gold))
                .Add(new Recruits(faction.Recruits));
            
            scenario.Players.Add(playerEntity);

            world.Spawn().Add(new SpawnUnitEvent(spawnEvent.Side, faction.Leaders[0], spawnEvent.Coords, true));
        }
    }
}