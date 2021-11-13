using Godot.Collections;
using Bitron.Ecs;

public struct SpawnPlayersEvent
{
    public Dictionary<int, string> Factions;

    public SpawnPlayersEvent(Dictionary<int, string> factions)
    {
        Factions = factions;
    }
}

public class SpawnPlayersEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var startingLocQuery = world.Query<IsStartingPositionOfTeam>().End();
        var eventQuery = world.Query<SpawnPlayersEvent>().End();

        foreach (var e in eventQuery)
        {
            ref var spawnEvent = ref world.Entity(e).Get<SpawnPlayersEvent>();

            foreach (var locEntityId in startingLocQuery)
            {
                var locEntity = world.Entity(locEntityId);

                ref var coords = ref locEntity.Get<Coords>();
                ref var startPos = ref locEntity.Get<IsStartingPositionOfTeam>();

                world.Spawn().Add(new SpawnPlayerEvent(startPos.Value, coords, spawnEvent.Factions[startPos.Value], 100));
            }
        }
    }
}