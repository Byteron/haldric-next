using Bitron.Ecs;

struct HasBaseTerrain : IEcsAutoReset<HasBaseTerrain>
{
    public EcsEntity Entity;

    public HasBaseTerrain(EcsEntity terrainEntity)
    {
        Entity = terrainEntity;
    }

    public void AutoReset(ref HasBaseTerrain c)
    {
        c.Entity = default;
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
        c.Entity = default;
    }
}