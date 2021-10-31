using Bitron.Ecs;
using Godot;

public struct HighlightLocationEvent
{
    public Coords Coords;
    public int Range;

    public HighlightLocationEvent(Coords coords, int range)
    {
        Coords = coords;
        Range = range;
    }
}

public class HighlightLocationsEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<HighlightLocationEvent>().End();

        foreach (var eventEntityId in query)
        {
            var map = world.GetResource<Map>();
            ref var grid = ref map.Grid;

            var eventData = world.Entity(eventEntityId).Get<HighlightLocationEvent>();
            
            var locEntity = map.Locations.Get(eventData.Coords.Cube);
            var unitEntity = locEntity.Get<HasUnit>().Entity;

            ref var side = ref unitEntity.Get<Side>();
            ref var attacks = ref unitEntity.Get<Attacks>();
            
            var terrainHighlighter = world.GetResource<TerrainHighlighter>();
            terrainHighlighter.Clear();
            
            var maxAttackRange = attacks.GetMaxAttackRange();

            var cellsInMoveRange = Hex.GetCellsInRange(eventData.Coords.Cube, eventData.Range);
            var cellsInAttackRange = Hex.GetCellsInRange(eventData.Coords.Cube, maxAttackRange);

            foreach (var cCell in cellsInAttackRange)
            {
                var nCoords = Coords.FromCube(cCell);
                if (!grid.IsCoordsInGrid(nCoords))
                {
                    continue;
                }

                var nLocEntity = map.Locations.Dict[nCoords.Cube];

                var attackRange = map.GetEffectiveAttackDistance(eventData.Coords, nCoords);
                var attackerBonusAttackRange = map.GetBonusAttackRange(eventData.Coords, nCoords);

                var attack = attacks.GetUsableAttack(attackRange, attackerBonusAttackRange);

                ref var nElevation = ref nLocEntity.Get<Elevation>();
                
                var position = nCoords.World;
                position.y = nElevation.Height + 0.1f;
                
                var hasUnit = nLocEntity.Has<HasUnit>();

                if (hasUnit)
                {
                    if (nLocEntity.Get<HasUnit>().Entity.Get<Side>().Value == side.Value)
                    continue;
                }

                if (attack.IsAlive())
                {
                    if (attackRange > 1)
                    {
                        terrainHighlighter.PlaceHighlight(position, new Color("881111"), 0.6f);
                    }
                    else
                    {
                        terrainHighlighter.PlaceHighlight(position, new Color("774411"), 0.6f);
                    }
                }
            }

            foreach (var cCell in cellsInMoveRange)
            {
                var nCoords = Coords.FromCube(cCell);
                if (!grid.IsCoordsInGrid(nCoords))
                {
                    continue;
                }

                var nLocEntity = map.Locations.Dict[nCoords.Cube];

                if (nLocEntity.Get<Distance>().Value > eventData.Range)
                {
                    continue;
                }

                ref var nElevation = ref nLocEntity.Get<Elevation>();
                
                var position = nCoords.World;
                position.y = nElevation.Height + 0.1f;
                
                var hasUnit = nLocEntity.Has<HasUnit>();

                if (hasUnit)
                {
                    if (nLocEntity.Get<HasUnit>().Entity.Get<Side>().Value == side.Value)
                    continue;
                }

                if (hasUnit)
                {
                    continue;
                }
                
                terrainHighlighter.PlaceHighlight(position, new Color("111188"), 0.8f);
            }
        }
    }
}