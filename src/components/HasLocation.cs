using RelEcs;
using RelEcs.Godot;

public struct HasLocation : IReset<HasLocation>
{
    public Entity Entity { get; set; }

    public HasLocation(Entity entity)
    {
        Entity = entity;
    }

    public void Reset(ref HasLocation c)
    {
        c.Entity = default;
    }
}