using Godot;
using Bitron.Ecs;
using System.Collections.Generic;
using System.Linq;
using Haldric.Wdk;

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
        world.ForEach((ref UnitHoveredEvent e) =>
        {
            var unitEntity = e.UnitEntity;

            var id = unitEntity.Get<Id>().Value;
            var l = unitEntity.Get<Level>().Value;
            var hp = unitEntity.Get<Attribute<Health>>();
            var mp = unitEntity.Get<Attribute<Moves>>();
            var xp = unitEntity.Get<Attribute<Experience>>();

            var s = $"ID: {id}\nL: {l}\nHP: {hp}\nMP: {mp}\nXP: {xp}";

            if (unitEntity.Has<Attacks>())
            {
                s += "\nAttacks:";
                foreach (EcsEntity attackEntity in unitEntity.Get<Attacks>().List)
                {
                    ref var attackId = ref attackEntity.Get<Id>();
                    ref var damage = ref attackEntity.Get<Damage>();
                    ref var strikes = ref attackEntity.Get<Strikes>();
                    ref var range = ref attackEntity.Get<Range>();
                    s += string.Format("\n{0} {1}x{2}~{4} ({3})", attackId.Value, damage.Value, strikes.Value, damage.Type.ToString(), range.Value.ToString());
                }
            }

            var mobility = unitEntity.Get<Mobility>();

            if (mobility.Dict.Count > 0)
            {
                s += "\nMobility: ";
            }

            foreach (var item in mobility.Dict)
            {
                var terrainType = item.Key;
                var cost = item.Value;

                s += terrainType.ToString() + ": " + cost;
            }

            var weaknesses = unitEntity.Get<Weaknesses>();
            var resistances = unitEntity.Get<Resistances>();
            var calamities = unitEntity.Get<Calamities>();
            var immunities = unitEntity.Get<Immunities>();

            if (weaknesses.List.Count > 0)
            {
                s += "\nWeak To: " + string.Join(", ", weaknesses.List);
            }

            if (resistances.List.Count > 0)
            {
                s += "\nResistant To: " + string.Join(", ", resistances.List);
            }

            if (calamities.List.Count > 0)
            {
                s += "\nVery Weak To: " + string.Join(", ", calamities.List);
            }

            if (immunities.List.Count > 0)
            {
                s += "\nImmune To: " + string.Join(", ", immunities.List);
            }

            var unitPanel = world.GetResource<UnitPanel>();
            unitPanel.UpdateInfo(s);
        });
    }
}