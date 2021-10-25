using System.Collections.Generic;
using System.Linq;
using Godot;
using Bitron.Ecs;

public struct Distance
{
    public int Value;

    public Distance(int value)
    {
        Value = value;
    }
}

public struct PathFrom
{
    public EcsEntity LocEntity;

    public PathFrom(EcsEntity locEntity)
    {
        LocEntity = locEntity;
    }
}

public class Map
{
    public Grid Grid;
    public Locations Locations;
    public Vector2i ChunkSize;
    public PathFinder PathFinder;

    public Map(int width, int height, int chunkSize)
    {
        Grid = new Grid(width, height);
        Locations = new Locations();
        Locations.AutoReset(ref Locations);
        ChunkSize = new Vector2i(chunkSize, chunkSize);
        PathFinder = new PathFinder();
    }

    public Vector3 GetBeginPosition()
    {
        var coords = Coords.FromOffset(0, 0);
        return coords.World;
    }

    public Vector3 GetEndPosition()
    {
        var coords = Coords.FromOffset(Grid.Width - 1, Grid.Height - 1);
        return coords.World;
    }

    public Vector3 GetCenterPosition()
    {
        var coords = Coords.FromOffset(Grid.Width / 2, Grid.Height / 2);
        return coords.World; 
    }

    public void UpdateDistances(Coords fromCoords, int team)
    {
        foreach (var loc in Locations.Dict.Values)
        {
            loc.Get<Distance>().Value = int.MaxValue;
        }

        var fromLocEntity = Locations.Dict[fromCoords.Cube];
        
        fromLocEntity.Get<Distance>().Value = 0;

        List<EcsEntity> frontier = new List<EcsEntity>();
        frontier.Add(fromLocEntity);

        while (frontier.Count > 0)
        {
            // await Main.Instance.ToSignal(Main.Instance.GetTree(), "process_frame");

            var cLocEntity = frontier[0];
            frontier.RemoveAt(0);

            var cCoords = cLocEntity.Get<Coords>();

            var cDistance = cLocEntity.Get<Distance>().Value;
            var cElevation = cLocEntity.Get<Elevation>().Value;

            foreach(var nLocEntity in cLocEntity.Get<Neighbors>().GetArray())
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

                var nMovementCost = TerrainTypes.FromLocEntity(nLocEntity).GetMovementCost();

                if (cLocEntity.Has<HasUnit>() && cCoords.Cube != fromCoords.Cube)
                {
                    var unitEntity = cLocEntity.Get<HasUnit>().Entity;

                    if (unitEntity.Get<Team>().Value != team)
                    {
                        nMovementCost = 99;
                    }
                }

                var distance = cDistance + nMovementCost;
                
                ref var nDistance = ref nLocEntity.Get<Distance>();

                if (nDistance.Value == int.MaxValue)
                {
                    nDistance.Value = distance;
                    frontier.Add(nLocEntity);
                    // Main.Instance.World.Spawn().Add(new SpawnFloatingLabelEvent(nLocEntity.Get<Coords>().World, distance.ToString(), new Color(1f, 1f, 1f)));
                } 
                else if (distance < nDistance.Value)
                {
                    nDistance.Value = distance;
                    // Main.Instance.World.Spawn().Add(new SpawnFloatingLabelEvent(nLocEntity.Get<Coords>().World, distance.ToString(), new Color(1f, 1f, 1f)));
                }
                
                frontier.Sort((locA, locB) => locA.Get<Distance>().Value.CompareTo(locB.Get<Distance>().Value));
            }
        }
    }

    public Path FindPath(Coords fromCoords, Coords toCoords, int team)
    {
        foreach (var loc in Locations.Dict.Values)
        {
            loc.Get<Distance>().Value = int.MaxValue;
        }

        var fromLocEntity = Locations.Dict[fromCoords.Cube];
        var toLocEntity = Locations.Dict[toCoords.Cube];

        var path = new Path();
        path.Start = fromLocEntity;
        path.Destination = toLocEntity;
        
        fromLocEntity.Get<Distance>().Value = 0;

        List<EcsEntity> frontier = new List<EcsEntity>();
        frontier.Add(fromLocEntity);

        while (frontier.Count > 0)
        {
            // await Main.Instance.ToSignal(Main.Instance.GetTree(), "process_frame");

            var cLocEntity = frontier[0];
            frontier.RemoveAt(0);

            var cCoords = cLocEntity.Get<Coords>();

            if (cCoords.Cube == toCoords.Cube)
            {
                path.Checkpoints.Enqueue(cLocEntity);
             
                var current = cLocEntity.Get<PathFrom>().LocEntity;
                var cPathFrom = current.Get<PathFrom>();

				while (current.Get<Coords>().Cube != fromCoords.Cube)
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

            foreach(var nLocEntity in cLocEntity.Get<Neighbors>().GetArray())
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

                var nMovementCost = TerrainTypes.FromLocEntity(nLocEntity).GetMovementCost();

                if (cLocEntity.Has<HasUnit>() && cCoords.Cube != fromCoords.Cube)
                {
                    var unitEntity = cLocEntity.Get<HasUnit>().Entity;

                    if (unitEntity.Get<Team>().Value != team)
                    {
                        nMovementCost = 99;
                    }
                }

                var distance = cDistance + nMovementCost;
                
                ref var nDistance = ref nLocEntity.Get<Distance>();
                ref var nPathFrom = ref nLocEntity.Get<PathFrom>();

                if (nDistance.Value == int.MaxValue)
                {
                    nDistance.Value = distance;
                    nPathFrom.LocEntity = cLocEntity;
                    frontier.Add(nLocEntity);
                    // Main.Instance.World.Spawn().Add(new SpawnFloatingLabelEvent(nLocEntity.Get<Coords>().World, distance.ToString(), new Color(1f, 1f, 1f)));
                } 
                else if (distance < nDistance.Value)
                {
                    nDistance.Value = distance;
                    nPathFrom.LocEntity = cLocEntity;
                    // Main.Instance.World.Spawn().Add(new SpawnFloatingLabelEvent(nLocEntity.Get<Coords>().World, distance.ToString(), new Color(1f, 1f, 1f)));
                }
                
                frontier.Sort((locA, locB) => locA.Get<Distance>().Value.CompareTo(locB.Get<Distance>().Value));
            }
        }

        return path;
    }

    // public Path FindPath(Coords startCoords, Coords endCoords)
    // {
    //     var path = new Path();
        
    //     path.Start = Locations.Get(startCoords.Cube);
    //     path.Destination = Locations.Get(endCoords.Cube);

    //     Vector3[] pointPath = PathFinder.GetPointPath(startCoords.GetIndex(Grid.Width), endCoords.GetIndex(Grid.Width));

    //     foreach (var cell in pointPath)
    //     {
    //         path.Checkpoints.Enqueue(Locations.Get(cell));
    //     }

    //     if (path.Checkpoints.Count > 1)
    //     {
    //         path.Checkpoints.Dequeue();
    //     }

    //     return path;
    // }
}