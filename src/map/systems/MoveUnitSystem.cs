using Godot;
using Leopotam.Ecs;

public class MoveUnitSystem : IEcsRunSystem
{
    EcsFilter<MapCursor> _filter;
    EcsFilter<Commander> _commander;

    public void Run()
    {
        if (_filter.IsEmpty() || _commander.IsEmpty())
        {
            return;
        }

        ref var commander = ref _commander.GetEntity(0).Get<Commander>();

        var cursorEntity = _filter.GetEntity(0);
        var hoveredLocEntity = cursorEntity.Get<HoveredLocation>().Entity;
        
        if (hoveredLocEntity == EcsEntity.Null)
        {
            return;
        }

        if (!cursorEntity.Has<HasLocation>())
        {
            return;
        }

        ref var hasLocation = ref cursorEntity.Get<HasLocation>();
        var selectedLocEntity = hasLocation.Entity;

        if (Input.IsActionJustPressed("select_unit"))
        {
            commander.Enqueue(new MoveCommand(selectedLocEntity, hoveredLocEntity));
            hasLocation.Entity = hoveredLocEntity;
        }
    }
}