using Bitron.Ecs;
using Godot;

public struct AdvanceEvent
{
    public EcsEntity Entity;

    public AdvanceEvent(EcsEntity entity)
    {
        Entity = entity;
    }
}

public class AdvanceEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<AdvanceEvent>().End();

        foreach (var id in query)
        {
            var hudView = world.GetResource<HUDView>();
            
            ref var gainEvent = ref query.Get<AdvanceEvent>(id);

            var entity = gainEvent.Entity;

            if (!entity.Has<Level>())
            {
                continue;
            }

            ref var level = ref entity.Get<Level>();
            
            level.Value += 1;

            ref var coords = ref entity.Get<Coords>();

            hudView.SpawnFloatingLabel(coords.World + Vector3.Up * 8f, $"++{level.Value}++", new Color(1f, 1f, 0.6f));
        }
    }
}