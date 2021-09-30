using Godot;
using Bitron.Ecs;

public struct DestroyMapEvent { }

public class DestroyMapEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<DestroyMapEvent>().End();
        var mapQuery = world.Query<Map>().Inc<Locations>().End();
        var chunkQuery = world.Query<Chunk>().End();
        var cursorQuery = world.Query<MapCursor>().End();
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

            foreach (var entityId in cursorQuery)
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
        }
    }
}