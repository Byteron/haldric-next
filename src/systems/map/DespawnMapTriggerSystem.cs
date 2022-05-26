using Godot;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public class DespawnMapTrigger { }

public class DespawnMapTriggerSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((DespawnMapTrigger e) =>
        {
            var query = commands.Query<Entity>().Any<Locations>().Any<Location>().Any<Attribute<Health>>();

            foreach (var entity in query)
            {
                entity.Despawn();
            }

            commands.RemoveElement<Map>();
            commands.RemoveElement<ShaderData>();
            commands.RemoveElement<HoveredLocation>();

            commands.RemoveElement<Cursor3D>();

            commands.RemoveElement<TerrainHighlighter>();

            if (commands.HasElement<SelectedLocation>())
            {
                commands.RemoveElement<SelectedLocation>();
            }
        });
    }
}