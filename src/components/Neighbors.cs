using RelEcs;
using RelEcs.Godot;

public class Neighbors
{
    public Entity[] Array { get; } = new Entity[6];

    public bool Has(Direction direction)
    {
        var entity = Array[(int)direction];
        return entity is not null && entity.IsAlive;
    }

    public Entity Get(Direction direction)
    {
        return Array[(int)direction];
    }

    public void Set(Direction direction, Entity entity)
    {
        Array[(int)direction] = entity;
    }
}
