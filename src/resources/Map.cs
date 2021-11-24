using System.Collections.Generic;
using System.Linq;
using Godot;
using Bitron.Ecs;
using Haldric.Wdk;

public struct IsInZoc { }

public struct Distance
{
    public int Value { get; set; }

    public Distance(int value)
    {
        Value = value;
    }
}

public struct PathFrom
{
    public EcsEntity LocEntity { get; set; }

    public PathFrom(EcsEntity locEntity)
    {
        LocEntity = locEntity;
    }
}

public class Map
{
    public Grid Grid { get; set; }
    public Locations Locations { get; set; }
    public Vector2i ChunkSize { get; set; }
    public PathFinder PathFinder { get; set; }

    public Map(int width, int height, int chunkSize)
    {
        Grid = new Grid(width, height);
        Locations = new Locations()
        {
            Dict = new Dictionary<Vector3, EcsEntity>()
        };
        ChunkSize = new Vector2i(chunkSize, chunkSize);
        PathFinder = new PathFinder();
    }

    public Vector3 GetBeginPosition()
    {
        var coords = Coords.FromOffset(0, 0);
        return coords.World();
    }

    public Vector3 GetEndPosition()
    {
        var coords = Coords.FromOffset(Grid.Width - 1, Grid.Height - 1);
        return coords.World();
    }

    public Vector3 GetCenterPosition()
    {
        var coords = Coords.FromOffset(Grid.Width / 2, Grid.Height / 2);
        return coords.World();
    }

    public void UpdateDistances(Coords fromCoords, int side)
    {
        foreach (var loc in Locations.Dict.Values)
        {
            loc.Get<Distance>().Value = int.MaxValue;

            if (loc.Has<IsInZoc>())
            {
                loc.Remove<IsInZoc>();
            }
        }

        foreach (var loc in Locations.Dict.Values)
        {
            if (!loc.Has<HasUnit>())
            {
                continue;
            }

            var unitEntity = loc.Get<HasUnit>().Entity;

            if (unitEntity.Get<Side>().Value != side)
            {
                ref var neighbors = ref loc.Get<Neighbors>();

                foreach (var nLoc in neighbors.Array)
                {
                    if (!nLoc.IsAlive() || nLoc.Has<IsInZoc>() || nLoc.Has<HasUnit>())
                    {
                        continue;
                    }

                    nLoc.Add<IsInZoc>();
                }
            }
        }

        var fromLocEntity = Locations.Dict[fromCoords.Cube()];
        
        Mobility mobility = new Mobility();
        mobility.Dict = new Dictionary<TerrainType, int>();

        if (fromLocEntity.Has<HasUnit>())
        {
            var unitEntity = fromLocEntity.Get<HasUnit>().Entity;
            mobility = unitEntity.Get<Mobility>();
        }

        fromLocEntity.Get<Distance>().Value = 0;

        List<EcsEntity> frontier = new List<EcsEntity>();
        frontier.Add(fromLocEntity);

        while (frontier.Count > 0)
        {
            var cLocEntity = frontier[0];
            frontier.RemoveAt(0);

            var cCoords = cLocEntity.Get<Coords>();

            var cDistance = cLocEntity.Get<Distance>().Value;
            var cElevation = cLocEntity.Get<Elevation>().Value;

            foreach (var nLocEntity in cLocEntity.Get<Neighbors>().Array)
            {
                if (!nLocEntity.IsAlive())
                {
                    continue;
                }

                var nElevation = nLocEntity.Get<Elevation>().Value;
                var elevationDifference = Mathf.Abs(cElevation - nElevation);

                if (elevationDifference > 1)
                {
                    continue;
                }

                var nMovementCost = TerrainTypes.FromLocEntity(nLocEntity).GetMovementCost(mobility);

                if (cLocEntity.Has<HasUnit>() && cCoords.Cube() != fromCoords.Cube())
                {
                    var unitEntity = cLocEntity.Get<HasUnit>().Entity;

                    if (unitEntity.Get<Side>().Value != side)
                    {
                        nMovementCost = 99;
                    }
                }

                if (cLocEntity.Has<IsInZoc>())
                {
                    nMovementCost = 99;
                }

                var distance = cDistance + nMovementCost;

                ref var nDistance = ref nLocEntity.Get<Distance>();

                if (nDistance.Value == int.MaxValue)
                {
                    nDistance.Value = distance;
                    frontier.Add(nLocEntity);
                }
                else if (distance < nDistance.Value)
                {
                    nDistance.Value = distance;
                }

                frontier.Sort((locA, locB) => locA.Get<Distance>().Value.CompareTo(locB.Get<Distance>().Value));
            }
        }
    }

    public bool IsInMeleeRange(Coords fromCoords, Coords toCoords)
    {
        var fromLocEntity = Locations.Get(fromCoords.Cube());
        var toLocEntity = Locations.Get(toCoords.Cube());

        ref var fromElevation = ref fromLocEntity.Get<Elevation>();
        ref var toElevation = ref toLocEntity.Get<Elevation>();

        var distance = Hex.GetDistance(fromCoords.Cube(), toCoords.Cube());
        var diff = Mathf.Abs(fromElevation.Value - toElevation.Value);

        if (distance > 1 || diff > 1)
        {
            return false;
        }

        return true;
    }

    public int GetAttackDistance(Coords fromCoords, Coords toCoords)
    {
        return Hex.GetDistance(fromCoords.Cube(), toCoords.Cube());
    }

    public Path FindPath(Coords fromCoords, Coords toCoords, int side)
    {
        foreach (var loc in Locations.Dict.Values)
        {
            loc.Get<Distance>().Value = int.MaxValue;

            if (loc.Has<IsInZoc>())
            {
                loc.Remove<IsInZoc>();
            }
        }

        foreach (var loc in Locations.Dict.Values)
        {
            if (!loc.Has<HasUnit>())
            {
                continue;
            }

            var unitEntity = loc.Get<HasUnit>().Entity;

            if (unitEntity.Get<Side>().Value != side)
            {
                ref var neighbors = ref loc.Get<Neighbors>();

                foreach (var nLoc in neighbors.Array)
                {
                    if (!nLoc.IsAlive() || nLoc.Has<IsInZoc>() || nLoc.Has<HasUnit>())
                    {
                        continue;
                    }

                    nLoc.Add<IsInZoc>();
                }
            }
        }

        var fromLocEntity = Locations.Dict[fromCoords.Cube()];
        var toLocEntity = Locations.Dict[toCoords.Cube()];

        var path = new Path();
        path.Start = fromLocEntity;
        path.Destination = toLocEntity;

        if (fromCoords.Cube() == toCoords.Cube())
        {
            return path;
        }

        fromLocEntity.Get<Distance>().Value = 0;

        Mobility mobility = new Mobility();
        mobility.Dict = new Dictionary<TerrainType, int>();

        if (fromLocEntity.Has<HasUnit>())
        {
            var unitEntity = fromLocEntity.Get<HasUnit>().Entity;
            mobility = unitEntity.Get<Mobility>();
        }

        List<EcsEntity> frontier = new List<EcsEntity>();
        frontier.Add(fromLocEntity);

        while (frontier.Count > 0)
        {
            var cLocEntity = frontier[0];
            frontier.RemoveAt(0);

            var cCoords = cLocEntity.Get<Coords>();

            if (cCoords.Cube() == toCoords.Cube())
            {
                path.Checkpoints.Enqueue(cLocEntity);

                var current = cLocEntity.Get<PathFrom>().LocEntity;
                var cPathFrom = current.Get<PathFrom>();

                while (current.Get<Coords>().Cube() != fromCoords.Cube())
                {
                    path.Checkpoints.Enqueue(current);
                    current = cPathFrom.LocEntity;
                    cPathFrom = current.Get<PathFrom>();
                }

                path.Checkpoints = new Queue<EcsEntity>(path.Checkpoints.Reverse());
                break;
            }

            var cDistance = cLocEntity.Get<Distance>().Value;
            var cElevation = cLocEntity.Get<Elevation>().Value;

            foreach (var nLocEntity in cLocEntity.Get<Neighbors>().Array)
            {
                if (!nLocEntity.IsAlive())
                {
                    continue;
                }

                var nElevation = nLocEntity.Get<Elevation>().Value;
                var elevationDifference = Mathf.Abs(cElevation - nElevation);

                if (elevationDifference > 1)
                {
                    continue;
                }

                var nMovementCost = TerrainTypes.FromLocEntity(nLocEntity).GetMovementCost(mobility);

                if (cLocEntity.Has<HasUnit>() && cCoords.Cube() != fromCoords.Cube())
                {
                    var unitEntity = cLocEntity.Get<HasUnit>().Entity;

                    if (unitEntity.Get<Side>().Value != side)
                    {
                        nMovementCost = 99;
                    }
                }

                if (cLocEntity.Has<IsInZoc>())
                {
                    nMovementCost = 99;
                }

                var distance = cDistance + nMovementCost;

                ref var nDistance = ref nLocEntity.Get<Distance>();
                ref var nPathFrom = ref nLocEntity.Get<PathFrom>();

                if (nDistance.Value == int.MaxValue)
                {
                    nDistance.Value = distance;
                    nPathFrom.LocEntity = cLocEntity;
                    frontier.Add(nLocEntity);
                }
                else if (distance < nDistance.Value)
                {
                    nDistance.Value = distance;
                    nPathFrom.LocEntity = cLocEntity;
                }

                frontier.Sort((locA, locB) => locA.Get<Distance>().Value.CompareTo(locB.Get<Distance>().Value));
            }
        }

        return path;
    }

    private List<EcsEntity> GetLocEntitiesFromCubes(Vector3[] cubes)
    {
        List<EcsEntity> list = new List<EcsEntity>();

        foreach (var cube in cubes)
        {
            if (Locations.Dict.ContainsKey(cube))
            {
                list.Add(Locations.Dict[cube]);
            }
        }

        return list;
    }
}