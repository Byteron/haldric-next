using Leopotam.Ecs;

public struct HasLocation : IEcsAutoReset<HasLocation>
{
    public EcsEntity Entity;

    public HasLocation(EcsEntity entity)
    {
        Entity = entity;
    }
    
    public void AutoReset(ref HasLocation c)
    {
        c.Entity = EcsEntity.Null;
    }
}