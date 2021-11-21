using Godot;
using Bitron.Ecs;

public struct UnitDeselectedEvent { }

public class UnitDeselectedEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<UnitDeselectedEvent>().End();

        foreach (var _ in query)
        {
            if (world.HasResource<SelectedLocation>())
            {
                world.RemoveResource<SelectedLocation>();

                var terrainHighlighter = world.GetResource<TerrainHighlighter>();
                terrainHighlighter.Clear();

                var unitPanel = world.GetResource<UnitPanel>();
                unitPanel.UpdateInfo("");
            }
        }
    }
}