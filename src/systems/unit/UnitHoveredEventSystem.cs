using Godot;
using Bitron.Ecs;

public struct UnitHoveredEvent
{
    public EcsEntity UnitEntity;
    
    public UnitHoveredEvent(EcsEntity unitEntity)
    {
        UnitEntity = unitEntity;
    }
}

public class UnitHoveredEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<UnitHoveredEvent>().End();

        foreach (var eventEntityId in query)
        {
            var unitEntity = world.Entity(eventEntityId).Get<UnitHoveredEvent>().UnitEntity;

            var id = unitEntity.Get<Id>().Value;
            var hp = unitEntity.Get<Attribute<Health>>();
            var ap = unitEntity.Get<Attribute<Moves>>();
            var xp = unitEntity.Get<Attribute<Experience>>();

            var s = string.Format("ID: {0}\nHP: {1}\nAP: {2}\nXP: {3}", id, hp.ToString(), ap.ToString(), xp.ToString());

            if (unitEntity.Has<Attacks>())
            {
                s += "\nAttacks:";
                foreach(EcsEntity attackEntity in unitEntity.Get<Attacks>().GetList())
                {
                    ref var attackId = ref attackEntity.Get<Id>();
                    ref var damage = ref attackEntity.Get<Damage>();
                    ref var strikes = ref attackEntity.Get<Strikes>();
                    ref var range = ref attackEntity.Get<Range>();
                    s += string.Format("\n{0} {1}x{2}~{4} ({3})", attackId.Value, damage.Value, strikes.Value, damage.Type.ToString(), range.Value.ToString());
                }
            }

            var hudView = world.GetResource<HUDView>();
            hudView.UnitLabel.Text = s;
        }
    }
}