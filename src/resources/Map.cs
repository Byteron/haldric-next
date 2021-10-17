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

    public Queue<EcsEntity> FindPath(Coords startCoords, Coords endCoords)
    {
        var pathQueue = new Queue<EcsEntity>();
        Vector3[] path = PathFinder.GetPointPath(startCoords.GetIndex(Grid.Width), endCoords.GetIndex(Grid.Width));

        foreach (var cell in path)
        {
            pathQueue.Enqueue(Locations.Get(cell));
        }

        return pathQueue;
    }
}