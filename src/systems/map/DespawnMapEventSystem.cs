using Godot;
using Bitron.Ecs;
using Haldric.Wdk;

public struct DespawnMapEvent { }

public class DespawnMapEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<DespawnMapEvent>().End();
        var chunkQuery = world.Query<Locations>().End();
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

            foreach (var locEntity in map.Locations.Values)
            {
                locEntity.Despawn();
            }
            
            world.RemoveResource<Map>();
            world.RemoveResource<ShaderData>();
            world.RemoveResource<HoveredLocation>();

            var cursor = world.GetResource<Cursor3D>();
            cursor.QueueFree();
            world.RemoveResource<Cursor3D>();

            var highlighter = world.GetResource<TerrainHighlighter>();
            highlighter.QueueFree();
            world.RemoveResource<TerrainHighlighter>();

            if (world.HasResource<SelectedLocation>())
            {
                world.RemoveResource<SelectedLocation>();
            }
        }
    }
}