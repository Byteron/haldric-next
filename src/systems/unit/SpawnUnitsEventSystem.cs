using Bitron.Ecs;

public struct SpawnUnitsEvent { }

public class SpawnUnitsEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var keepQuery = world.Query<Keep>().End();
        var eventQuery = world.Query<SpawnUnitsEvent>().End();

        foreach (var e in eventQuery)
        {
            Godot.GD.Print("Spawn Units Event Received");
            
            int playerId = 0;
            foreach (var locEntityId in keepQuery)
            {
                Godot.GD.Print("Keep Found");

                var locEntity = world.Entity(locEntityId);
                ref var keep = ref locEntity.Get<Keep>();

                world.Spawn().Add(new SpawnUnitEvent(playerId, "Cavalry", locEntity.Get<Coords>()));

                foreach (var castleLoc in keep.List)
                {
                    world.Spawn().Add(new SpawnUnitEvent(playerId, "Spearman", castleLoc.Get<Coords>()));
                }

                playerId += 1;
            }
        }
    }
}