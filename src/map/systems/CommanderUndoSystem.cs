using Godot;
using Bitron.Ecs;

public class CommanderUndoSystem : IEcsSystem
{
    EcsFilter<Commander> _filter;

    public void Run(EcsWorld world)
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