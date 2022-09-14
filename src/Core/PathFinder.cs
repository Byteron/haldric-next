using Godot;
using RelEcs;
using System.Collections.Generic;

public class Path
{
    public Entity Start { get; set; }
    public Entity Destination { get; set; }

    public Queue<Entity> Checkpoints { get; set; } = new();
}

public partial class PathFinder : AStar3D
{
    public override double _ComputeCost(long fromId, long toId) { return 1.0f; }
    public override double _EstimateCost(long fromId, long toId) { return 1.0f; }
}