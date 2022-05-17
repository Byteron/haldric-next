using System.Collections.Generic;
using Bitron.Ecs;

public struct SpawnScenarioEvent
{
    public int Round;
    public int Side;
    public Dictionary<int, EcsEntity> Sides;
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

            var scenario = new Scenario();
            scenario.Round = spawnEvent.Round;
            scenario.Side = spawnEvent.Side;
            scenario.Sides = spawnEvent.Sides;

            world.AddResource(scenario);
        }
    }
}