using RelEcs;
using RelEcs.Godot;

public class HasLocation
{
    public Entity Entity { get; set; }

    public HasLocation(Entity entity)
    {
        Entity = entity;
    }
}