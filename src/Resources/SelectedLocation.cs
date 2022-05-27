using RelEcs;
using RelEcs.Godot;

public class SelectedLocation
{
    public Entity Entity { get; set; }

    public SelectedLocation(Entity entity)
    {
        Entity = entity;
    }
}