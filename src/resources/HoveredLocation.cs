using Bitron.Ecs;

public class HoveredLocation
{
    public EcsEntity Entity { get; set; } = default;
    public bool HasChanged { get; set; } = false;
}