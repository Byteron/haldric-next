using Godot;
using RelEcs;
using System.Collections.Generic;

public class Path
{
    public Entity Start { get; set; }
    public Entity Destination { get; set; }

    public Queue<Entity> Checkpoints { get; set; } = new();

    public int GetCost() { return Destination.Get<Distance>().Value; }

    public override string ToString()
    {
        return
            $"From: {Start.Get<Coords>().Cube()}, To: {Start.Get<Coords>().Cube()}, Checkpoints: {Checkpoints.Count}";
    }
}

public partial class PathFinder : AStar3D
{
    public override float _ComputeCost(int fromId, int toId) { return 1.0f; }
    public override float _EstimateCost(int fromId, int toId) { return 1.0f; }
}