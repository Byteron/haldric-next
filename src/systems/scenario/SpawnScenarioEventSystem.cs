using System.Collections.Generic;
using Bitron.Ecs;

public struct SpawnScenarioEvent
{
    public int Round;
    public int Side;
    public List<EcsEntity> Players;
}

public class SpawnScenarioEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<SpawnScenarioEvent>().End();

        foreach (var eventId in query)
        {
            var eventEntity = world.Entity(eventId);

            ref var spawnEvent = ref eventEntity.Get<SpawnScenarioEvent>();

            var scenario = new Scenario(spawnEvent.Players.Count);
            scenario.Round = spawnEvent.Round;
            scenario.Side = spawnEvent.Side;
            scenario.Players = spawnEvent.Players.ToArray();

            world.AddResource(scenario);
        }
    }
}