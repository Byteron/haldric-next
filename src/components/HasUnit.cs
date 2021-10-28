using Bitron.Ecs;

public struct HasUnit : IEcsAutoReset<HasUnit>
{
    public EcsEntity Entity { get; set; }

    public HasUnit(EcsEntity entity)
    {
        Entity = entity;
    }

    public void AutoReset(ref HasUnit c)
    {
        c.Entity = default;
    }
}