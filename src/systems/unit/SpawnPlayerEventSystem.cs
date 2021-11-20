using System.Collections.Generic;
using Bitron.Ecs;

public struct SpawnPlayerEvent
{
    public string Id;
    public int Side;
    public Coords Coords;
    public string Faction;
    public int Gold;

    public SpawnPlayerEvent(int side, string id, Coords coords, string faction, int gold)
    {
        Id = id;
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
                .Add(new Id(spawnEvent.Id))
                .Add(new Side(spawnEvent.Side))
                .Add(new Gold(spawnEvent.Gold))
                .Add(new Recruits(faction.Recruits));

            scenario.Players[spawnEvent.Side] = playerEntity;

            world.Spawn().Add(new SpawnUnitEvent(spawnEvent.Side, faction.Leaders[0], spawnEvent.Coords, true));
        }
    }
}