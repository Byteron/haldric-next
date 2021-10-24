using Godot;
using Bitron.Ecs;

public class SelectTargetSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<HoveredLocation>().End();

        foreach (var hoverEntityId in query)
        {
            var hoverEntity = world.Entity(hoverEntityId);
            var hoveredLocEntity = hoverEntity.Get<HoveredLocation>().Entity;

            if (!hoveredLocEntity.IsAlive())
            {
                return;
            }

            if (Input.IsActionJustPressed("select_unit"))
            {
                if (hoveredLocEntity.Has<HasUnit>() && hoverEntity.Has<HasLocation>())
                {
                    var selectedLocEntity = hoverEntity.Get<HasLocation>().Entity;

                    if (!selectedLocEntity.Has<HasUnit>())
                    {
                        return;
                    }

                    var attackerUnitEntity = selectedLocEntity.Get<HasUnit>().Entity;

                    ref var actions = ref attackerUnitEntity.Get<Attribute<Actions>>();

                    if (actions.IsEmpty())
                    {
                        return;
                    }

                    var defenderUnitEntity = hoveredLocEntity.Get<HasUnit>().Entity;

                    ref var attackerAttacks = ref attackerUnitEntity.Get<Attacks>();
                    ref var defenderAttacks = ref defenderUnitEntity.Get<Attacks>();

                    if (attackerUnitEntity.Get<Team>().Value == defenderUnitEntity.Get<Team>().Value)
                    {
                        return;
                    }

                    ref var moves = ref attackerUnitEntity.Get<Attribute<Moves>>();

                    var distance = selectedLocEntity.Get<Coords>().DistanceTo(defenderUnitEntity.Get<Coords>());
                    bool canAttack = attackerAttacks.HasUsableAttack(distance);

                    if (!canAttack)
                    {
                        return;
                    }

                    var commander = world.GetResource<Commander>();
                    var gameStateController = world.GetResource<GameStateController>();

                    var attackerAttackEntity = attackerAttacks.GetUsableAttack(distance);
                    var defenderAttackEntity = defenderAttacks.GetUsableAttack(distance);

                    commander.Enqueue(new CombatCommand(attackerUnitEntity, attackerAttackEntity, defenderUnitEntity, defenderAttackEntity));
                    gameStateController.PushState(new CommanderState(world));
                }
            }
        }
    }
}