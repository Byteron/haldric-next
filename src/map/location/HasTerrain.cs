using Leopotam.Ecs;

struct HasBaseTerrain : IEcsAutoReset<HasBaseTerrain>
{
    public EcsEntity Entity;

    public HasBaseTerrain(EcsEntity terrainEntity)
    {
        Entity = terrainEntity;
    }

    public void AutoReset(ref HasBaseTerrain c)
    {
        if (c.Entity != EcsEntity.Null)
        {
            c.Entity.Destroy();
        }
        
        c.Entity = EcsEntity.Null;
    }
}

struct HasOverlayTerrain : IEcsAutoReset<HasOverlayTerrain>
{
    public EcsEntity Entity;

    public HasOverlayTerrain(EcsEntity terrainEntity)
    {
        Entity = terrainEntity;
    }

    public void AutoReset(ref HasOverlayTerrain c)
    {
        if (c.Entity != EcsEntity.Null)
        {
            c.Entity.Destroy();
        }
        
        c.Entity = EcsEntity.Null;
    }
}