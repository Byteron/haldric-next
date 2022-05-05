using Godot;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public struct DespawnMapEvent { }

public class DespawnMapEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((DespawnMapEvent e) =>
        {
            commands.ForEach((Entity entity, ref Locations locs) =>
            {
                entity.Despawn();
            });

            commands.ForEach((Entity entity, ref Location loc) =>
            {
                entity.Despawn();
            });

            commands.ForEach((Entity entity, ref Attribute<Health> health) =>
            {
                entity.Despawn();
            });

            commands.RemoveElement<Map>();
            commands.RemoveElement<ShaderData>();
            commands.RemoveElement<HoveredLocation>();

            var cursor = commands.GetElement<Cursor3D>();
            commands.RemoveElement<Cursor3D>();

            var highlighter = commands.GetElement<TerrainHighlighter>();
            commands.RemoveElement<TerrainHighlighter>();

            if (commands.HasElement<SelectedLocation>())
            {
                commands.RemoveElement<SelectedLocation>();
            }
        });
    }
}