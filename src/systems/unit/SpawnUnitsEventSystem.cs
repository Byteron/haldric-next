using Bitron.Ecs;

public struct SpawnUnitsEvent { }

public class SpawnUnitsEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var keepQuery = world.Query<Castle>().End();
        var eventQuery = world.Query<SpawnUnitsEvent>().End();

        var scenario = world.GetResource<Scenario>();

        foreach (var e in eventQuery)
        {
            int playerId = 0;
            foreach (var locEntityId in keepQuery)
            {

                var locEntity = world.Entity(locEntityId);
                ref var keep = ref locEntity.Get<Castle>();

                world.Spawn().Add(new SpawnUnitEvent(playerId, "Cavalry", locEntity.Get<Coords>()));

                foreach (var castleLoc in keep.List)
                {
                    if (Godot.GD.Randf() < 0.5f)
                    {
                        world.Spawn().Add(new SpawnUnitEvent(playerId, "Spearman", castleLoc.Get<Coords>()));
                    }
                    else
                    {
                        world.Spawn().Add(new SpawnUnitEvent(playerId, "Bowman", castleLoc.Get<Coords>()));
                    }
                }

                playerId += 1;
            }
            
            scenario.PlayerCount = playerId;
        }

    }
}