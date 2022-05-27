using System.Collections.Generic;
using Godot;
using RelEcs;
using RelEcs.Godot;

public class SelectTargetSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<SelectedLocation>(out var selectedLocation)) return;
        if (!commands.TryGetElement<HoveredLocation>(out var hoveredLocation)) return;

        var defenderLocEntity = hoveredLocation.Entity;

        if (!defenderLocEntity.IsAlive) return;

        if (!Input.IsActionJustPressed("select_unit") || !defenderLocEntity.Has<HasUnit>()) return;
        var attackerLocEntity = selectedLocation.Entity;

        if (!attackerLocEntity.Has<HasUnit>()) return;

        var attackerUnitEntity = attackerLocEntity.Get<HasUnit>().Entity;

        var actions = attackerUnitEntity.Get<Attribute<Actions>>();

        if (actions.IsEmpty()) return;

        var defenderUnitEntity = defenderLocEntity.Get<HasUnit>().Entity;

        if (attackerUnitEntity.Get<Side>().Value == defenderUnitEntity.Get<Side>().Value) return;

        var attackerCoords = attackerLocEntity.Get<Coords>();
        var defenderCoords = defenderLocEntity.Get<Coords>();

        var attackerAttacks = attackerUnitEntity.Get<Attacks>();
        var defenderAttacks = defenderUnitEntity.Get<Attacks>();

        var map = commands.GetElement<Map>();

        var isInMeleeRange = map.IsInMeleeRange(attackerCoords, defenderCoords);
        var attackDistance = Map.GetAttackDistance(attackerCoords, defenderCoords);

        var attackerUsableAttacks = attackerAttacks.GetUsableAttacks(isInMeleeRange, attackDistance);

        var attackPairs = new Dictionary<Entity, Entity>();

        foreach (var attackerAttackEntity in attackerUsableAttacks)
        {
            var defenderAttackEntity = defenderAttacks.GetUsableAttack(isInMeleeRange, attackDistance);
            attackPairs.Add(attackerAttackEntity, defenderAttackEntity);
        }

        if (attackPairs.Count == 0) return;

        var gameStateController = commands.GetElement<GameStateController>();

        var attackSelectionState = new AttackSelectionState();

        attackSelectionState.AttackerLocEntity = attackerLocEntity;
        attackSelectionState.DefenderLocEntity = defenderLocEntity;
        attackSelectionState.AttackPairs = attackPairs;
        attackSelectionState.AttackDistance = attackDistance;
        gameStateController.PushState(attackSelectionState);
    }
}