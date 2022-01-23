using Godot;
using Bitron.Ecs;
using Haldric.Wdk;

public struct DespawnMapEvent { }

public class DespawnMapEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        world.ForEach((ref DespawnMapEvent e) =>
        {
            world.ForEach((EcsEntity entity, ref Locations locs) =>
            {
                entity.Despawn();
            });

            world.ForEach((EcsEntity entity, ref Location loc) =>
            {
                entity.Despawn();
            });

            world.ForEach((EcsEntity entity, ref Attribute<Health> health) =>
            {
                entity.Despawn();
            });

            world.RemoveResource<Map>();
            world.RemoveResource<ShaderData>();
            world.RemoveResource<HoveredLocation>();

            var cursor = world.GetResource<Cursor3D>();
            world.RemoveResource<Cursor3D>();

            var highlighter = world.GetResource<TerrainHighlighter>();
            world.RemoveResource<TerrainHighlighter>();

            if (world.HasResource<SelectedLocation>())
            {
                world.RemoveResource<SelectedLocation>();
            }
        });
    }
}