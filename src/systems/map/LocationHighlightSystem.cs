using Godot;
using Bitron.Ecs;

public class LocationHighlightSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var hoverQuery = world.Query<HoveredLocation>()
            .Inc<Highlighter>()
            .Inc<NodeHandle<Cursor3D>>()
            .End();
        
        foreach(var cursorEntityId in hoverQuery)
        {
            var locEntity = hoverQuery.Get<HoveredLocation>(cursorEntityId).Entity;

            if (!locEntity.IsAlive())
            {
                continue;
            }
            
            var view = hoverQuery.Get<NodeHandle<Cursor3D>>(cursorEntityId).Node;

            ref var coords = ref locEntity.Get<Coords>();
            var height = locEntity.Get<Elevation>().Height;

            var position = coords.World;
            position.y = height;

            GD.Print(coords.ToString());

            view.Position = position;
        }
        
    }
}