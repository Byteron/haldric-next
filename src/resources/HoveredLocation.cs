using RelEcs;
using RelEcs.Godot;

public class HoveredLocation
{
    public Entity Entity { get; set; } = default;
    public bool HasChanged { get; set; } = false;
}