using System.Collections.Generic;
using Bitron.Ecs;
using Haldric.Wdk;

public struct Mobility : IEcsAutoReset<Mobility>
{
    public Dictionary<TerrainType, int> Dict { get; set; }

    public void AutoReset(ref Mobility c)
    {
        if (c.Dict == null)
        {
            c.Dict = new Dictionary<TerrainType, int>();
        }

        c.Dict.Clear();
    }
}