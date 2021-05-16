using Godot;
using Leopotam.Ecs;

public class CommanderUndoSystem : IEcsRunSystem
{
    EcsFilter<Commander> _filter;

    public void Run()
    {
        foreach (var i in _filter)
        {
            var commanderEntity = _filter.GetEntity(i);
            ref var commander = ref commanderEntity.Get<Commander>();

            if (Input.IsActionJustPressed("undo"))
            {
                commander.Undo();
            }
        }
    }
}