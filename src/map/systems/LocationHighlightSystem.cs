using Godot;
using Bitron.Ecs;

struct Highlighter {}

public class LocationHighlightSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var cursorQuery = world.Query<HoveredLocation>().Inc<Highlighter>().Inc<NodeHandle<Node3D>>().End();
        
        foreach(var cursorEntityId in cursorQuery)
        {
            var locEntity = cursorQuery.Get<HoveredLocation>(cursorEntityId).Entity;
            var view = cursorQuery.Get<NodeHandle<Node3D>>(cursorEntityId).Node;

            ref var coords = ref locEntity.Get<Coords>();
            var height = locEntity.Get<Elevation>().Height;

            var position = coords.World;
            position.y = height;

            GD.Print(coords.ToString());

            view.Position = position;
        }
        
    }
}