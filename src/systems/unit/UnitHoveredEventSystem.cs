using Godot;
using Bitron.Ecs;

public struct UnitHoveredEvent
{
    public EcsEntity UnitEntity { get; set; }

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
            var l = unitEntity.Get<Level>().Value;
            var hp = unitEntity.Get<Attribute<Health>>();
            var ap = unitEntity.Get<Attribute<Actions>>();
            var mp = unitEntity.Get<Attribute<Moves>>();
            var xp = unitEntity.Get<Attribute<Experience>>();

            var s = $"ID: {id}\nL: {l}\nHP: {hp}\nAP: {ap}\nMP: {mp}\nXP: {xp}";

            if (unitEntity.Has<Attacks>())
            {
                s += "\nAttacks:";
                foreach(EcsEntity attackEntity in unitEntity.Get<Attacks>().List)
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