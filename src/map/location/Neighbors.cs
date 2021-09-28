using Bitron.Ecs;

public struct Neighbors
{
    private EcsPackedEntity[] _array;

    public bool Has(Direction direction)
    {
        return _array[(int)direction] != default;
    }

    public EcsPackedEntity Get(Direction direction)
    {
        return _array[(int)direction];
    }

    public void Set(Direction direction, EcsPackedEntity entity)
    {
        _array[(int)direction] = entity;
    }

    public EcsPackedEntity[] GetArray()
    {
        return _array;
    }
}