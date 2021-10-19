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

    public Path FindPath(Coords startCoords, Coords endCoords)
    {
        var path = new Path();
        
        path.Start = Locations.Get(endCoords.Cube);
        path.Destination = Locations.Get(startCoords.Cube);

        Vector3[] pointPath = PathFinder.GetPointPath(startCoords.GetIndex(Grid.Width), endCoords.GetIndex(Grid.Width));

        foreach (var cell in pointPath)
        {
            path.Checkpoints.Enqueue(Locations.Get(cell));
        }

        return path;
    }
}