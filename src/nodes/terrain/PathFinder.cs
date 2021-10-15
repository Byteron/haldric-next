using Godot;
using Bitron.Ecs;

public partial class PathFinder : AStar2D
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
