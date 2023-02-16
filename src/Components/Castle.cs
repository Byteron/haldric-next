using System.Collections.Generic;
using RelEcs;

public class Castle
{
    public List<Entity> List { get; set; } = new();

    // public bool TryGetFreeLoc(out Entity locEntity)
    // {
    //     foreach (var cLocEntity in List)
    //     {
    //         if (!cLocEntity.Has<HasUnit>())
    //         {
    //             locEntity = cLocEntity;
    //             return true;
    //         }
    //     }

    //     locEntity = default;
    //     return false;
    // }

    // public bool IsLocFree(Coords coords)
    // {
    //     foreach (var cLocEntity in List)
    //     {
    //         if (!cLocEntity.Has<HasUnit>() && cLocEntity.Get<Coords>().Cube() == coords.ToCube())
    //         {
    //             return true;
    //         }
    //     }

    //     return false;
    // }
}