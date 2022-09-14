using System.Collections.Generic;
using Godot;
using RelEcs;

public class IsInZoc
{
}

public class Distance
{
    public int Value { get; set; }

    public Distance() => Value = 0;
}

public class PathFrom
{
    public Entity LocEntity { get; set; }
    public PathFrom() => LocEntity = null;
}

public class Map
{
    public Grid Grid { get; }
    public Tiles Tiles { get; }
    public Vector2i ChunkSize { get; }
    public PathFinder PathFinder { get; }

    public Map(int width, int height, int chunkSize)
    {
        Grid = new Grid(width, height);
        Tiles = new Tiles { Dict = new Dictionary<Vector3, Entity>() };
        ChunkSize = new Vector2i(chunkSize, chunkSize);
        PathFinder = new PathFinder();
    }

    public static Vector3 GetBeginPosition()
    {
        var coords = Coords.FromOffset(0, 0);
        return coords.ToWorld();
    }

    public Vector3 GetEndPosition()
    {
        var coords = Coords.FromOffset(Grid.Width - 1, Grid.Height - 1);
        return coords.ToWorld();
    }

    // public void UpdateDistances(Coords fromCoords, int side)
    // {
    //     foreach (var loc in Tiles.Dict.Values)
    //     {
    //         loc.Get<Distance>().Value = int.MaxValue;

    //         if (loc.Has<IsInZoc>()) loc.Remove<IsInZoc>();
    //     }

    //     foreach (var loc in Tiles.Dict.Values)
    //     {
    //         if (!loc.Has<HasUnit>()) continue;

    //         var unitEntity = loc.Get<HasUnit>().Entity;

    //         if (unitEntity.Get<Side>().Value == side) continue;

    //         var neighbors = loc.Get<Neighbors>();

    //         foreach (var nLoc in neighbors.Array)
    //         {
    //             if (!nLoc.IsAlive || nLoc.Has<IsInZoc>() || nLoc.Has<HasUnit>()) continue;
    //             nLoc.Add<IsInZoc>();
    //         }
    //     }

    //     var fromLocEntity = Tiles.Dict[fromCoords.Cube()];

    //     var mobility = new Mobility
    //     {
    //         Dict = new Dictionary<TerrainType, int>()
    //     };

    //     if (fromLocEntity.Has<HasUnit>())
    //     {
    //         var unitEntity = fromLocEntity.Get<HasUnit>().Entity;
    //         mobility = unitEntity.Get<Mobility>();
    //     }

    //     fromLocEntity.Get<Distance>().Value = 0;

    //     var frontier = new List<Entity> { fromLocEntity };

    //     while (frontier.Count > 0)
    //     {
    //         var cLocEntity = frontier[0];
    //         frontier.RemoveAt(0);

    //         var cCoords = cLocEntity.Get<Coords>();

    //         var cDistance = cLocEntity.Get<Distance>().Value;
    //         var cElevation = cLocEntity.Get<Elevation>().Value;

    //         foreach (var nLocEntity in cLocEntity.Get<Neighbors>().Array)
    //         {
    //             if (nLocEntity is null || !nLocEntity.IsAlive) continue;

    //             var nElevation = nLocEntity.Get<Elevation>().Value;
    //             var elevationDifference = Mathf.Abs(cElevation - nElevation);

    //             if (elevationDifference > 1) continue;

    //             var nMovementCost = TerrainTypes.FromLocEntity(nLocEntity).GetMovementCost(mobility);

    //             if (cLocEntity.Has<HasUnit>() && cCoords.Cube() != fromCoords.Cube())
    //             {
    //                 var unitEntity = cLocEntity.Get<HasUnit>().Entity;

    //                 if (unitEntity.Get<Side>().Value != side) nMovementCost = 99;
    //             }

    //             if (cLocEntity.Has<IsInZoc>()) nMovementCost = 99;

    //             var distance = cDistance + nMovementCost;

    //             var nDistance = nLocEntity.Get<Distance>();

    //             if (nDistance.Value == int.MaxValue)
    //             {
    //                 nDistance.Value = distance;
    //                 frontier.Add(nLocEntity);
    //             }
    //             else if (distance < nDistance.Value)
    //             {
    //                 nDistance.Value = distance;
    //             }

    //             frontier.Sort((locA, locB) => locA.Get<Distance>().Value.CompareTo(locB.Get<Distance>().Value));
    //         }
    //     }
    // }

    // public bool IsInMeleeRange(Coords fromCoords, Coords toCoords)
    // {
    //     var fromLocEntity = Tiles.Get(fromCoords.Cube());
    //     var toLocEntity = Tiles.Get(toCoords.Cube());

    //     var fromElevation = fromLocEntity.Get<Elevation>();
    //     var toElevation = toLocEntity.Get<Elevation>();

    //     var distance = Hex.GetDistance(fromCoords.Cube(), toCoords.Cube());
    //     var diff = Mathf.Abs(fromElevation.Value - toElevation.Value);

    //     return distance <= 1 && diff <= 1;
    // }

    // public static int GetAttackDistance(Coords fromCoords, Coords toCoords)
    // {
    //     return Hex.GetDistance(fromCoords.Cube(), toCoords.Cube());
    // }

    // public Path FindPath(Coords fromCoords, Coords toCoords, int side)
    // {
    //     foreach (var loc in Tiles.Dict.Values)
    //     {
    //         loc.Get<Distance>().Value = int.MaxValue;

    //         if (loc.Has<IsInZoc>()) loc.Remove<IsInZoc>();
    //     }

    //     foreach (var loc in Tiles.Dict.Values)
    //     {
    //         if (!loc.Has<HasUnit>()) continue;

    //         var unitEntity = loc.Get<HasUnit>().Entity;

    //         if (unitEntity.Get<Side>().Value == side) continue;

    //         var neighbors = loc.Get<Neighbors>();

    //         foreach (var nLoc in neighbors.Array)
    //         {
    //             if (!nLoc.IsAlive || nLoc.Has<IsInZoc>() || nLoc.Has<HasUnit>()) continue;
    //             nLoc.Add<IsInZoc>();
    //         }
    //     }

    //     var fromLocEntity = Tiles.Dict[fromCoords.Cube()];
    //     var toLocEntity = Tiles.Dict[toCoords.Cube()];

    //     var path = new Path
    //     {
    //         Start = fromLocEntity,
    //         Destination = toLocEntity
    //     };

    //     if (fromCoords.Cube() == toCoords.Cube())
    //     {
    //         return path;
    //     }

    //     fromLocEntity.Get<Distance>().Value = 0;

    //     var mobility = new Mobility
    //     {
    //         Dict = new Dictionary<TerrainType, int>()
    //     };

    //     if (fromLocEntity.Has<HasUnit>())
    //     {
    //         var unitEntity = fromLocEntity.Get<HasUnit>().Entity;
    //         mobility = unitEntity.Get<Mobility>();
    //     }

    //     var frontier = new List<Entity> { fromLocEntity };

    //     while (frontier.Count > 0)
    //     {
    //         var cLocEntity = frontier[0];
    //         frontier.RemoveAt(0);

    //         var cCoords = cLocEntity.Get<Coords>();

    //         if (cCoords.Cube() == toCoords.Cube())
    //         {
    //             path.Checkpoints.Enqueue(cLocEntity);

    //             var current = cLocEntity.Get<PathFrom>().LocEntity;
    //             var cPathFrom = current.Get<PathFrom>();

    //             while (current.Get<Coords>().Cube() != fromCoords.Cube())
    //             {
    //                 path.Checkpoints.Enqueue(current);
    //                 current = cPathFrom.LocEntity;
    //                 cPathFrom = current.Get<PathFrom>();
    //             }

    //             path.Checkpoints = new Queue<Entity>(path.Checkpoints.Reverse());
    //             break;
    //         }

    //         var cDistance = cLocEntity.Get<Distance>().Value;
    //         var cElevation = cLocEntity.Get<Elevation>().Value;

    //         foreach (var nLocEntity in cLocEntity.Get<Neighbors>().Array)
    //         {
    //             if (nLocEntity is null || !nLocEntity.IsAlive) continue;

    //             var nElevation = nLocEntity.Get<Elevation>().Value;
    //             var elevationDifference = Mathf.Abs(cElevation - nElevation);

    //             if (elevationDifference > 1) continue;

    //             var nMovementCost = TerrainTypes.FromLocEntity(nLocEntity).GetMovementCost(mobility);

    //             if (cLocEntity.Has<HasUnit>() && cCoords.Cube() != fromCoords.Cube())
    //             {
    //                 var unitEntity = cLocEntity.Get<HasUnit>().Entity;

    //                 if (unitEntity.Get<Side>().Value != side) nMovementCost = 99;
    //             }

    //             if (cLocEntity.Has<IsInZoc>()) nMovementCost = 99;

    //             var distance = cDistance + nMovementCost;

    //             var nDistance = nLocEntity.Get<Distance>();
    //             var nPathFrom = nLocEntity.Get<PathFrom>();

    //             if (nDistance.Value == int.MaxValue)
    //             {
    //                 nDistance.Value = distance;
    //                 nPathFrom.LocEntity = cLocEntity;
    //                 frontier.Add(nLocEntity);
    //             }
    //             else if (distance < nDistance.Value)
    //             {
    //                 nDistance.Value = distance;
    //                 nPathFrom.LocEntity = cLocEntity;
    //             }

    //             frontier.Sort((locA, locB) => locA.Get<Distance>().Value.CompareTo(locB.Get<Distance>().Value));
    //         }
    //     }

    //     return path;
    // }

    List<Entity> GetLocEntitiesFromCubes(Vector3[] cubes)
    {
        var list = new List<Entity>();

        foreach (var cube in cubes)
        {
            if (Tiles.Dict.ContainsKey(cube))
            {
                list.Add(Tiles.Dict[cube]);
            }
        }

        return list;
    }
}