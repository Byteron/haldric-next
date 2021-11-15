using System.Collections.Generic;
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

            if (attackerUnitEntity.Get<Side>().Value == defenderUnitEntity.Get<Side>().Value)
            {
                return;
            }

            ref var attackerCoords = ref attackerLocEntity.Get<Coords>();
            ref var defenderCoords = ref defenderLocEntity.Get<Coords>();

            ref var attackerAttacks = ref attackerUnitEntity.Get<Attacks>();
            ref var defenderAttacks = ref defenderUnitEntity.Get<Attacks>();

            var map = world.GetResource<Map>();

            var isInMeleeRange = map.IsInMeleeRange(attackerCoords, defenderCoords);
            var attackDistance = map.GetAttackDistance(attackerCoords, defenderCoords);

            var attackerUsableAttacks = attackerAttacks.GetUsableAttacks(isInMeleeRange, attackDistance);

            var attackPairs = new Dictionary<EcsEntity, EcsEntity>();

            foreach (var attackerAttackEntity in attackerUsableAttacks)
            {
                var defenderAttackEntity = defenderAttacks.GetUsableAttack(isInMeleeRange, attackDistance);
                attackPairs.Add(attackerAttackEntity, defenderAttackEntity);
            }

            if (attackPairs.Count == 0)
            {
                return;
            }

            var gameStateController = world.GetResource<GameStateController>();

            var attackSelectionState = new AttackSelectionState(world);

            attackSelectionState.AttackerLocEntity = attackerLocEntity;
            attackSelectionState.DefenderLocEntity = defenderLocEntity;
            attackSelectionState.AttackPairs = attackPairs;
            attackSelectionState.AttackDistance = attackDistance;
            gameStateController.PushState(attackSelectionState);
        }
    }
}