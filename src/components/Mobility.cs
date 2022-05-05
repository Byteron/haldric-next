using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public struct Mobility : IReset<Mobility>
{
    public Dictionary<TerrainType, int> Dict { get; set; }

    public void Reset(ref Mobility c)
    {
        if (c.Dict == null)
        {
            c.Dict = new Dictionary<TerrainType, int>();
        }

        c.Dict.Clear();
    }
}