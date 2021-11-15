using Bitron.Ecs;
using Godot;
using System.Collections.Generic;

public struct HighlightLocationEvent
{
    public Coords Coords { get; set; }
    public int Range { get; set; }

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
            var grid = map.Grid;

            var eventData = world.Entity(eventEntityId).Get<HighlightLocationEvent>();

            var locEntity = map.Locations.Get(eventData.Coords.Cube());
            ref var unit = ref locEntity.Get<HasUnit>();
            var unitEntity = unit.Entity;
            ref var side = ref unitEntity.Get<Side>();
            ref var attacks = ref unitEntity.Get<Attacks>();

            var terrainHighlighter = world.GetResource<TerrainHighlighter>();
            terrainHighlighter.Clear();

            var maxAttackRange = attacks.GetMaxAttackRange();
            var cellsInAttackRange = Hex.GetCellsInRange(eventData.Coords.Cube(), maxAttackRange);

            List<Coords> filteredAttackList = new List<Coords>();
            foreach (var cCell in cellsInAttackRange)
            {
                var nCoords = Coords.FromCube(cCell);
                if (!grid.IsCoordsInGrid(nCoords))
                {
                    continue;
                }

                var nLocEntity = map.Locations.Dict[nCoords.Cube()];

                var isInMeleeRange = map.IsInMeleeRange(eventData.Coords, nCoords);
                var attackRange = map.GetAttackDistance(eventData.Coords, nCoords);

                var attack = attacks.GetUsableAttack(isInMeleeRange, attackRange);

                var hasUnit = nLocEntity.Has<HasUnit>();

                if (attack.IsAlive() && hasUnit)
                {
                    if (nLocEntity.Get<HasUnit>().Entity.Get<Side>().Value == side.Value)
                    {
                        continue;
                    }

                    filteredAttackList.Add(nCoords);
                }
            }

            Vector3[] cellsInMoveRange = Hex.GetCellsInRange(eventData.Coords.Cube(), eventData.Range);
            List<Coords> filteredMoveList = new List<Coords>();

            foreach (Vector3 cCell in cellsInMoveRange)
            {
                Coords nCoords = Coords.FromCube(cCell);
                if (!grid.IsCoordsInGrid(nCoords))
                {
                    continue;
                }

                var nLocEntity = map.Locations.Dict[cCell];

                if (nLocEntity.Get<Distance>().Value > eventData.Range)
                {
                    continue;
                }

                var hasUnit = nLocEntity.Has<HasUnit>();

                if (hasUnit)
                {
                    continue;
                }
                
                filteredMoveList.Add(nCoords);
            }

            HighlightBorder(world, filteredAttackList, maxAttackRange, new Color("774411"), 0.9f);
            HighlightBorder(world, filteredMoveList, eventData.Range, new Color("111188"));
        }
    }

    private void HighlightBorder(EcsWorld world, List<Coords> locations, int range, Color color, float scaleFactor = 1f, bool debug = false)
    {
        var map = world.GetResource<Map>();
        var grid = map.Grid;
        var terrainHighlighter = world.GetResource<TerrainHighlighter>();

        foreach (Coords coord in locations)
        {
            Vector3 position = coord.World();
            Elevation elevation = map.Locations.Dict[coord.Cube()].Get<Elevation>();
            position.y = elevation.Height + 0.1f;
            Vector3[] Neighbors = Hex.GetNeighbors(coord.Cube());
            for (int i = 0; i < 6; i++)
            {
                Direction direction = (Direction)i;
                Vector3 cNeighbor = Neighbors[i];
                Coords nNeighbor = Coords.FromCube(cNeighbor);
                bool validCoord = grid.IsCoordsInGrid(nNeighbor);
                bool inRange = map.Locations.Dict[cNeighbor].Get<Distance>().Value <= range;
                bool inList = locations.Contains(nNeighbor);
                if (!validCoord || !inRange || !inList)
                {
                    terrainHighlighter.PlaceBorder(position, color, direction.Rotation(), scaleFactor);
                }
            }
        }
    }
}
