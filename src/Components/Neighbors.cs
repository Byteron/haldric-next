using RelEcs;

public class Neighbors
{
    public Entity[] Array =
    {
        Entity.None,
        Entity.None,
        Entity.None,
        Entity.None,
        Entity.None,
        Entity.None,
    };

    public bool Has(Direction direction)
    {
        return Array[(int)direction] != Entity.None;
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
