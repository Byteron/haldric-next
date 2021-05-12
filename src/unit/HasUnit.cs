using Leopotam.Ecs;

public struct HasUnit : IEcsAutoReset<HasUnit>
{
    public EcsEntity Entity;

    public HasUnit(EcsEntity entity)
    {
        Entity = entity;
    }

    public void AutoReset(ref HasUnit c)
    {
        c.Entity = EcsEntity.Null;
    }
}