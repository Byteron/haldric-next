using Godot;
using Bitron.Ecs;

public class SelectUnitSystem : IEcsSystem
{
    Node3D _parent;

    public SelectUnitSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run(EcsWorld world)
    {
        var query = world.Query<HoveredLocation>().End();

        foreach (var hoverEntityId in query)
        {
            var locEntity = query.Get<HoveredLocation>(hoverEntityId).Entity;
            
            if (!locEntity.IsAlive())
            {
                return;
            }

            if (Input.IsActionJustPressed("select_unit"))
            {
                var hoverEntity = world.Entity(hoverEntityId);

                if (locEntity.Has<HasUnit>())
                {
                    if (!hoverEntity.Has<HasLocation>())
                    {
                        hoverEntity.Add(new HasLocation(locEntity));
                    }
                    
                    var unitEntity = locEntity.Get<HasUnit>().Entity;
                    world.Spawn().Add(new UnitSelectedEvent(unitEntity));
                }
            }
        }
    }
}