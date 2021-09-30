using System.Collections.Generic;
using Bitron.Ecs;

public struct Keep : IEcsAutoReset<Keep>
{
    public List<EcsEntity> List;

    public void AutoReset(ref Keep c)
    {
        c.List = new List<EcsEntity>();
    }
}