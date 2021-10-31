using Bitron.Ecs;

public struct Neighbors : IEcsAutoReset<Neighbors>
{
    public EcsEntity[] Array { get; private set; }

    public bool Has(Direction direction)
    {
        return Array[(int)direction].IsAlive();
    }

    public EcsEntity Get(Direction direction)
    {
        return Array[(int)direction];
    }

    public void Set(Direction direction, EcsEntity entity)
    {
        Array[(int)direction] = entity;
    }

    public void AutoReset(ref Neighbors c)
    {
        c.Array = new EcsEntity[6];
    }
}
