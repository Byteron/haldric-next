using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;

public struct Castle : IReset<Castle>
{
    public List<Entity> List { get; set; }

    public bool TryGetFreeLoc(out Entity locEntity)
    {
        foreach (var cLocEntity in List)
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

    public void Reset(ref Castle c)
    {
        c.List = new List<Entity>();
    }
}