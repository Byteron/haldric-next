using Bitron.Ecs;

public struct Neighbors : IEcsAutoReset<Neighbors>
{
    private EcsEntity[] _array;

    public bool Has(Direction direction)
    {
        return _array[(int)direction].IsAlive();
    }

    public EcsEntity Get(Direction direction)
    {
        return _array[(int)direction];
    }

    public void Set(Direction direction, EcsEntity entity)
    {
        _array[(int)direction] = entity;
    }

    public EcsEntity[] GetArray()
    {
        return _array;
    }

    public void AutoReset(ref Neighbors c)
    {
        c._array = new EcsEntity[6];
    }
}