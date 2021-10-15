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

                        var unitEntity = hoveredLocEntity.Get<HasUnit>().Entity;

                        if (selectedUnitEntity.Get<Team>().Value != unitEntity.Get<Team>().Value)
                        {
                            world.Spawn().Add(new CombatEvent(selectedUnitEntity, unitEntity));
                        }
                    }
                    else
                    {
                        hoverEntity.Add(new HasLocation(hoveredLocEntity));

                        var unitEntity = hoveredLocEntity.Get<HasUnit>().Entity;
                        var scenario = world.GetResource<Scenario>();

                        if (unitEntity.Get<Team>().Value == scenario.CurrentPlayer)
                        {
                            world.Spawn().Add(new UnitSelectedEvent(unitEntity));
                        }
                    }

                    
                }
            }
        }
    }
}