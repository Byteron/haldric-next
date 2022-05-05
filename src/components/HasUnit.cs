using RelEcs;
using RelEcs.Godot;

public struct HasUnit : IReset<HasUnit>
{
    public Entity Entity { get; set; }

    public HasUnit(Entity entity)
    {
        Entity = entity;
    }

    public void Reset(ref HasUnit c)
    {
        c.Entity = default;
    }
}