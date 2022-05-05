using RelEcs;
using RelEcs.Godot;

public struct Neighbors : IReset<Neighbors>
{
    public Entity[] Array { get; private set; }

    public bool Has(Direction direction)
    {
        return Array[(int)direction].IsAlive;
    }

    public Entity Get(Direction direction)
    {
        return Array[(int)direction];
    }

    public void Set(Direction direction, Entity entity)
    {
        Array[(int)direction] = entity;
    }

    public void Reset(ref Neighbors c)
    {
        c.Array = new Entity[6];
    }
}
