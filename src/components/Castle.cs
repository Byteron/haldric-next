using System.Collections.Generic;
using Bitron.Ecs;

public struct Castle : IEcsAutoReset<Castle>
{
    public List<EcsEntity> List { get; set; }

    public bool TryGetFreeLoc(out EcsEntity locEntity)
    {
        foreach(var cLocEntity in List)
        {
            if (!cLocEntity.Has<HasUnit>())
            {
                locEntity = cLocEntity;
                return true;
            }
        }

        locEntity = default;
        return false;
    }

    public bool IsLocFree(Coords coords)
    {
        foreach (var cLocEntity in List)
        {
            if (!cLocEntity.Has<HasUnit>() && cLocEntity.Get<Coords>().Cube() == coords.Cube())
            {
                return true;
            }
        }

        return false;
    }

    public void AutoReset(ref Castle c)
    {
        c.List = new List<EcsEntity>();
    }
}