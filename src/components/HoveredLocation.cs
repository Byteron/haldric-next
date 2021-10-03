using Bitron.Ecs;

public struct Highlighter {}

public struct HoveredLocation : IEcsAutoReset<HoveredLocation>
{
    public EcsEntity Entity;
    public bool HasChanged;

    public HoveredLocation(EcsEntity entity)
    {
        Entity = entity;
        HasChanged = true;
    }
    
    public void AutoReset(ref HoveredLocation c)
    {
        c.Entity = default;
        c.HasChanged = false;
    }
}