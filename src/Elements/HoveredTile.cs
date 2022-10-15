using RelEcs;

public class HoveredTile
{
    public Entity Entity { get; set; }
    public Coords Coords { get; set; }
    public bool HasChanged { get; set; }
}