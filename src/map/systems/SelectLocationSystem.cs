using Godot;
using Leopotam.Ecs;

public class SelectLocationSystem : IEcsRunSystem
{
    EcsFilter<MapCursor> _filter;

    Node3D _parent;

    public SelectLocationSystem(Node3D parent)
    {
        _parent = parent;
    }

    public void Run()
    {
        if (_filter.IsEmpty())
        {
            return;
        }

        var cursorEntity = _filter.GetEntity(0);
        ref var locEntity = ref cursorEntity.Get<HoveredLocation>().Entity;
        
        if (locEntity == EcsEntity.Null)
        {
            return;
        }

        if (Input.IsActionJustPressed("select_unit"))
        {
            if (locEntity.Has<HasUnit>())
            {
                cursorEntity.Replace(new HasLocation(locEntity));

                var unitEntity = locEntity.Get<HasUnit>().Entity;
                var name = unitEntity.Get<NodeHandle<UnitView>>().Node.Name;

                _parent.GetTree().CallGroup("UnitLabel", "set", "text", "Unit Selected: " + name);
            }
        }

        if (Input.IsActionJustPressed("deselect_unit"))
        {
            cursorEntity.Del<HasLocation>();
            _parent.GetTree().CallGroup("UnitLabel", "set", "text", "");
        }
    }
}