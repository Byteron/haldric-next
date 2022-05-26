using RelEcs;
using RelEcs.Godot;
using Godot;
using System.Collections.Generic;

public class HighlightLocationEvent
{
    public Coords Coords { get; set; }
    public int Range { get; set; }

    public HighlightLocationEvent(Coords coords, int range)
    {
        Coords = coords;
        Range = range;
    }
}

public class HighlightLocationsEventSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.Receive((HighlightLocationEvent highlightEvent) =>
        {
            var map = commands.GetElement<Map>();
            var grid = map.Grid;

            var locEntity = map.Locations.Get(highlightEvent.Coords.Cube());
            var unit = locEntity.Get<HasUnit>();
            var unitEntity = unit.Entity;
            var side = unitEntity.Get<Side>();
            var attacks = unitEntity.Get<Attacks>();

            var terrainHighlighter = commands.GetElement<TerrainHighlighter>();
            terrainHighlighter.Clear();

            var maxAttackRange = attacks.GetMaxAttackRange();
            var cellsInAttackRange = Hex.GetCellsInRange(highlightEvent.Coords.Cube(), maxAttackRange);

            List<Coords> filteredAttackList = new List<Coords>();
            foreach (var cCell in cellsInAttackRange)
            {
                var nCoords = Coords.FromCube(cCell);
                if (!grid.IsCoordsInGrid(nCoords))
                {
                    continue;
                }

                var nLocEntity = map.Locations.Dict[nCoords.Cube()];

                var isInMeleeRange = map.IsInMeleeRange(highlightEvent.Coords, nCoords);
                var attackRange = Map.GetAttackDistance(highlightEvent.Coords, nCoords);

                var attack = attacks.GetUsableAttack(isInMeleeRange, attackRange);

                var hasUnit = nLocEntity.Has<HasUnit>();

                if (attack.IsAlive && hasUnit)
                {
                    if (nLocEntity.Get<HasUnit>().Entity.Get<Side>().Value == side.Value)
                    {
                        continue;
                    }

                    filteredAttackList.Add(nCoords);
                }
            }

            Vector3[] cellsInMoveRange = Hex.GetCellsInRange(highlightEvent.Coords.Cube(), highlightEvent.Range);
            List<Coords> filteredMoveList = new List<Coords>();

            foreach (Vector3 cCell in cellsInMoveRange)
            {
                Coords nCoords = Coords.FromCube(cCell);
                if (!grid.IsCoordsInGrid(nCoords))
                {
                    continue;
                }

                var nLocEntity = map.Locations.Dict[cCell];

                if (nLocEntity.Get<Distance>().Value > highlightEvent.Range)
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

            HighlightBorder(commands, filteredAttackList, maxAttackRange, new Color("774411"), 0.9f);
            HighlightBorder(commands, filteredMoveList, highlightEvent.Range, new Color("111188"));
        });
    }

     void HighlightBorder(Commands commands, List<Coords> locations, int range, Color color, float scaleFactor = 1f, bool debug = false)
    {
        var map = commands.GetElement<Map>();
        var grid = map.Grid;
        var terrainHighlighter = commands.GetElement<TerrainHighlighter>();

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
                bool inList = locations.Contains(nNeighbor);
                bool inRange = false;
                if (validCoord)
                {
                    inRange = map.Locations.Dict[cNeighbor].Get<Distance>().Value <= range;
                }
                if (!validCoord || !inRange || !inList)
                {
                    terrainHighlighter.PlaceBorder(position, color, Mathf.Tau - direction.Rotation(), scaleFactor);
                }
            }
        }
    }
}
