using Godot;
using Leopotam.Ecs;

public partial class AStarHex : AStar2D
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

public struct PathFinder : IEcsAutoReset<PathFinder>
{
    AStarHex AStar;

    public PathFinder(int width, int height)
    {
        AStar = new AStarHex();
        
    }

    public void AutoReset(ref PathFinder c)
    {
        c.AStar = new AStarHex();
    }
}