using Godot;
using Bitron.Ecs;
using System.Collections.Generic;

public class Path
{
    public EcsEntity Start;
    public EcsEntity Destination;

    public Queue<EcsEntity> Checkpoints = new Queue<EcsEntity>();

    
    public override string ToString()
    {
        return $"From: {Start.Get<Coords>().Cube}, To: {Start.Get<Coords>().Cube}, Checkpoints: {Checkpoints.Count}";
    }
}

public partial class PathFinder : AStar
{
    public override float _ComputeCost(int fromId, int toId)
    {
        return 1.0f;
    }

    public override float _EstimateCost(int fromId, int toId)
    {
        return 1.0f;
    }
}
