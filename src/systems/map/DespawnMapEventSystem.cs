using Godot;
using Bitron.Ecs;

public struct DespawnMapEvent { }

public class DespawnMapEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<DespawnMapEvent>().End();
        var chunkQuery = world.Query<Locations>().End();
        var hoverQuery = world.Query<HoveredLocation>().End();
        var unitQuery = world.Query<NodeHandle<UnitView>>().End();

        foreach (var _ in eventQuery)
        {
            var map = world.GetResource<Map>();
            
            foreach (var entityId in chunkQuery)
            {
                world.DespawnEntity(entityId);
            }

            foreach (var entityId in unitQuery)
            {
                world.DespawnEntity(entityId);
            }

            foreach (var entityId in hoverQuery)
            {
                world.DespawnEntity(entityId);
            }

            foreach (var locEntity in map.Locations.Values)
            {
                locEntity.Despawn();
            }
            
            world.RemoveResource<Map>();
            world.RemoveResource<ShaderData>();
        }
    }
}