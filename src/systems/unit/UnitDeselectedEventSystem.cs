using Godot;
using Bitron.Ecs;

public struct UnitDeselectedEvent { }

public class UnitDeselectedEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<HoveredLocation>().End();
        var eventQuery = world.Query<UnitDeselectedEvent>().End();

        foreach (var eventEntityId in eventQuery)
        {
            foreach (var hoverEntityId in query)
            {
                var hoverEntity = world.Entity(hoverEntityId);
                
                hoverEntity.Remove<HasLocation>();

                var hudView = world.GetResource<HUDView>();
                hudView.UnitLabel.Text = "";

                var terrainHighlighter = world.GetResource<TerrainHighlighter>();
                terrainHighlighter.Clear();
            }

        }
    }
}