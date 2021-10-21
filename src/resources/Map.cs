using System.Collections.Generic;
using Godot;
using Bitron.Ecs;

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

    public Path FindPath(Coords startCoords, Coords endCoords)
    {
        var path = new Path();
        
        path.Start = Locations.Get(startCoords.Cube);
        path.Destination = Locations.Get(endCoords.Cube);

        Vector3[] pointPath = PathFinder.GetPointPath(startCoords.GetIndex(Grid.Width), endCoords.GetIndex(Grid.Width));

        foreach (var cell in pointPath)
        {
            path.Checkpoints.Enqueue(Locations.Get(cell));
        }

        return path;
    }
}