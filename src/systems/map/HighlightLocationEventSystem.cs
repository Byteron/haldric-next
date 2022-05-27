using RelEcs;
using RelEcs.Godot;
using Godot;
using System.Collections.Generic;

public class HighlightLocationEvent
{
    public Coords Coords { get; }
    public int Range { get; }

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

            var filteredAttackList = new List<Coords>();
            foreach (var cCell in cellsInAttackRange)
            {
                var nCoords = Coords.FromCube(cCell);
                if (!grid.IsCoordsInGrid(nCoords)) continue;

                var nLocEntity = map.Locations.Dict[nCoords.Cube()];

                var isInMeleeRange = map.IsInMeleeRange(highlightEvent.Coords, nCoords);
                var attackRange = Map.GetAttackDistance(highlightEvent.Coords, nCoords);

                var attack = attacks.GetUsableAttack(isInMeleeRange, attackRange);

                var hasUnit = nLocEntity.Has<HasUnit>();

                if (attack is null || !attack.IsAlive || !hasUnit) continue;

                if (nLocEntity.Get<HasUnit>().Entity.Get<Side>().Value == side.Value) continue;

                filteredAttackList.Add(nCoords);
            }

            var cellsInMoveRange = Hex.GetCellsInRange(highlightEvent.Coords.Cube(), highlightEvent.Range);
            var filteredMoveList = new List<Coords>();

            foreach (var cCell in cellsInMoveRange)
            {
                var nCoords = Coords.FromCube(cCell);
                
                if (!grid.IsCoordsInGrid(nCoords)) continue;

                var nLocEntity = map.Locations.Dict[cCell];

                if (nLocEntity.Get<Distance>().Value > highlightEvent.Range) continue;

                var hasUnit = nLocEntity.Has<HasUnit>();

                if (hasUnit) continue;

                filteredMoveList.Add(nCoords);
            }

            HighlightBorder(commands, filteredAttackList, maxAttackRange, new Color("774411"), 0.9f);
            HighlightBorder(commands, filteredMoveList, highlightEvent.Range, new Color("111188"));
        });
    }

    static void HighlightBorder(Commands commands, List<Coords> locations, int range, Color color, float scaleFactor = 1f)
    {
        var map = commands.GetElement<Map>();
        var grid = map.Grid;
        var terrainHighlighter = commands.GetElement<TerrainHighlighter>();

        foreach (var coord in locations)
        {
            var position = coord.World();
            var elevation = map.Locations.Dict[coord.Cube()].Get<Elevation>();
            position.y = elevation.Height + 0.1f;
            var neighbors = Hex.GetNeighbors(coord.Cube());
            for (var i = 0; i < 6; i++)
            {
                var direction = (Direction)i;
                var cNeighbor = neighbors[i];
                var nNeighbor = Coords.FromCube(cNeighbor);
                var validCoord = grid.IsCoordsInGrid(nNeighbor);
                var inList = locations.Contains(nNeighbor);
                var inRange = false;
                
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