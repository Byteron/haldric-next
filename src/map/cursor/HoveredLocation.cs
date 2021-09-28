using Bitron.Ecs;

public struct HoveredLocation : IEcsAutoReset<HoveredLocation>
{
    public EcsEntity Entity;

    public HoveredLocation(EcsEntity entity)
    {
        Entity = entity;
    }
    
    public void AutoReset(ref HoveredLocation c)
    {
        c.Entity = EcsEntity.Null;
    }
}