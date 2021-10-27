using Godot;
using Bitron.Ecs;

public class SelectTargetSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.TryGetResource<SelectedLocation>(out var selectedLocation))
        {
            return;
        }

        if (!world.TryGetResource<HoveredLocation>(out var hoveredLocation))
        {
            return;
        }

        var defenderLocEntity = hoveredLocation.Entity;
        
        if (!defenderLocEntity.IsAlive())
        {
            return;
        }

        if (Input.IsActionJustPressed("select_unit") && defenderLocEntity.Has<HasUnit>())
        {
            var attackerLocEntity = selectedLocation.Entity;

            if (!attackerLocEntity.Has<HasUnit>())
            {
                return;
            }

            var attackerUnitEntity = attackerLocEntity.Get<HasUnit>().Entity;

            ref var actions = ref attackerUnitEntity.Get<Attribute<Actions>>();

            if (actions.IsEmpty())
            {
                return;
            }

            var defenderUnitEntity = defenderLocEntity.Get<HasUnit>().Entity;

            if (attackerUnitEntity.Get<Team>().Value == defenderUnitEntity.Get<Team>().Value)
            {
                return;
            }

            ref var attackerCoords = ref attackerLocEntity.Get<Coords>();
            ref var defenderCoords = ref defenderLocEntity.Get<Coords>();

            ref var attackerAttacks = ref attackerUnitEntity.Get<Attacks>();
            ref var defenderAttacks = ref defenderUnitEntity.Get<Attacks>();

            var map = world.GetResource<Map>();
            
            var attackDistance = map.GetEffectiveAttackDistance(attackerCoords, defenderCoords);
            var attackerAttackEntity = attackerAttacks.GetUsableAttack(attackDistance);

            if (!attackerAttackEntity.IsAlive())
            {
                return;
            }

            var commander = world.GetResource<Commander>();
            var gameStateController = world.GetResource<GameStateController>();

            var defenderAttackEntity = defenderAttacks.GetUsableAttack(attackDistance);

            commander.Enqueue(new CombatCommand(attackerLocEntity, attackerAttackEntity, defenderLocEntity, defenderAttackEntity));
            gameStateController.PushState(new CommanderState(world));
        }
    }
}