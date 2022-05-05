using RelEcs;
using RelEcs.Godot;

struct HasBaseTerrain : IReset<HasBaseTerrain>
{
    public Entity Entity { get; set; }

    public HasBaseTerrain(Entity terrainEntity)
    {
        Entity = terrainEntity;
    }

    public void Reset(ref HasBaseTerrain c)
    {
        c.Entity = default;
    }
}

struct HasOverlayTerrain : IReset<HasOverlayTerrain>
{
    public Entity Entity { get; set; }

    public HasOverlayTerrain(Entity terrainEntity)
    {
        Entity = terrainEntity;
    }

    public void Reset(ref HasOverlayTerrain c)
    {
        c.Entity = default;
    }
}