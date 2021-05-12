using Godot;
using Leopotam.Ecs;

public class SelectUnitSystem : IEcsRunSystem
{
    EcsFilter<MapCursor> _filter;

    Node3D _parent;

    public SelectUnitSystem(Node3D parent)
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
                var unitEntity = locEntity.Get<HasUnit>().Entity;

                var name = unitEntity.Get<NodeHandle<UnitView>>().Node.Name;

                cursorEntity.Replace(new HasUnit(unitEntity));
                _parent.GetTree().CallGroup("UnitLabel", "set", "text", "Unit Selected: " + name);
            }
        }

        if (Input.IsActionJustPressed("deselect_unit"))
        {
            cursorEntity.Del<HasUnit>();
            _parent.GetTree().CallGroup("UnitLabel", "set", "text", "");
        }
    }
}