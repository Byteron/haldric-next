using Bitron.Ecs;

public struct SpawnUnitsEvent { }

public class SpawnUnitsEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var keepQuery = world.Query<Castle>().End();
        var eventQuery = world.Query<SpawnUnitsEvent>().End();

        foreach (var e in eventQuery)
        {
            var scenario = world.GetResource<Scenario>();
            
            int playerId = 0;
            foreach (var locEntityId in keepQuery)
            {
                var playerEntity = world.Spawn()
                    .Add(new Team(playerId))
                    .Add(new Gold(100));
                
                scenario.Players.Add(playerEntity);

                var locEntity = world.Entity(locEntityId);
                ref var keep = ref locEntity.Get<Castle>();

                world.Spawn().Add(new SpawnUnitEvent(playerId, "Cavalry", locEntity.Get<Coords>()));

                foreach (var castleLoc in keep.List)
                {
                    if (castleLoc.Has<Castle>())
                    {
                        continue;
                    }

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
        }
    }
}