using Godot;
using Bitron.Ecs;

public class SelectUnitSystem : IEcsSystem
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
                if (hoveredLocEntity.Has<HasUnit>())
                {
                    if (hoverEntity.Has<HasLocation>())
                    {
                        var selectedLocEntity = hoverEntity.Get<HasLocation>().Entity;
                        var selectedUnitEntity = selectedLocEntity.Get<HasUnit>().Entity;

                        ref var actions = ref selectedUnitEntity.Get<Attribute<Actions>>();
                        var attackerAttackEntity = selectedUnitEntity.Get<Attacks>().GetFirst();
                        
                        if (actions.Value < attackerAttackEntity.Get<Costs>().Value)
                        {
                            return;
                        }

                        var unitEntity = hoveredLocEntity.Get<HasUnit>().Entity;

                        if (!selectedLocEntity.Get<Coords>().IsNeighborOf(unitEntity.Get<Coords>()))
                        {
                            return;
                        }

                        if (selectedUnitEntity.Get<Team>().Value == unitEntity.Get<Team>().Value)
                        {
                            return;
                        }

                        var commander = world.GetResource<Commander>();
                        var gameStateController = world.GetResource<GameStateController>();

                        commander.Enqueue(new CombatCommand(selectedUnitEntity, unitEntity));
                        gameStateController.PushState(new CommanderState(world));
                    }
                    else
                    {
                        var unitEntity = hoveredLocEntity.Get<HasUnit>().Entity;
                        var scenario = world.GetResource<Scenario>();

                        if (unitEntity.Get<Team>().Value == scenario.CurrentPlayer)
                        {
                            hoverEntity.Add(new HasLocation(hoveredLocEntity));
                            world.Spawn().Add(new UnitSelectedEvent(unitEntity));
                        }
                    }
                }
            }
        }
    }
}