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
            var defenderLocEntity = hoverEntity.Get<HoveredLocation>().Entity;
            
            if (!defenderLocEntity.IsAlive())
            {
                return;
            }

            if (Input.IsActionJustPressed("select_unit"))
            {
                if (defenderLocEntity.Has<HasUnit>() && hoverEntity.Has<HasLocation>())
                {
                    var attackerLocEntity = hoverEntity.Get<HasLocation>().Entity;

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
    }
}