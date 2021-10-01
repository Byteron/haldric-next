using Godot;
using Bitron.Ecs;

public struct DespawnMapEvent { }

public class DespawnMapEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<DespawnMapEvent>().End();
        var mapQuery = world.Query<Map>().Inc<Locations>().End();
        var chunkQuery = world.Query<Chunk>().End();
        var hoverQuery = world.Query<HoveredLocation>().End();
        var unitQuery = world.Query<NodeHandle<UnitView>>().End();

        foreach (var _ in eventQuery)
        {
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

            foreach (var entityId in mapQuery)
            {
                ref var locations = ref mapQuery.Get<Locations>(entityId);

                foreach (var locEntity in locations.Values)
                {
                    locEntity.Despawn();
                }

                world.DespawnEntity(entityId);
            }

            if (world.HasResource<ShaderData>())
            {
                world.RemoveResource<ShaderData>();
            }
        }


    }
}