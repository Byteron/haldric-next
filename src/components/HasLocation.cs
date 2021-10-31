using Bitron.Ecs;

public struct HasLocation : IEcsAutoReset<HasLocation>
{
    public EcsEntity Entity { get; set; }

    public HasLocation(EcsEntity entity)
    {
        Entity = entity;
    }
    
    public void AutoReset(ref HasLocation c)
    {
        c.Entity = default;
    }
}