using System.Collections.Generic;
using Bitron.Ecs;

public struct Castle : IEcsAutoReset<Castle>
{
    public List<EcsEntity> List;

    public void AutoReset(ref Castle c)
    {
        c.List = new List<EcsEntity>();
    }
}