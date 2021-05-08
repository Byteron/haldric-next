using Leopotam.Ecs;

struct HasTerrain : IEcsAutoReset<HasTerrain>
{
    public EcsEntity Entity;

    public HasTerrain(EcsEntity terrainEntity)
    {
        Entity = terrainEntity;
    }

    public void AutoReset(ref HasTerrain c)
    {
        if (c.Entity != EcsEntity.Null)
        {
            c.Entity.Destroy();
        }
        
        c.Entity = EcsEntity.Null;
    }
}