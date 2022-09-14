using System.Collections.Generic;
using RelEcs;

public class TerrainTypes
{
    public List<TerrainType> List { get; }
    
    public TerrainTypes() => List = new List<TerrainType>();
    public TerrainTypes(List<TerrainType> list) => List = list;

    // public static TerrainTypes FromLocEntity(Entity locEntity)
    // {
    //     var types = new TerrainTypes(new List<TerrainType>());

    //     var baseTerrain = locEntity.Get<HasBaseTerrain>().Entity;

    //     types.List.AddRange(baseTerrain.Get<TerrainTypes>().List);

    //     if (locEntity.Has<HasOverlayTerrain>())
    //     {
    //         var overlayTerrain = locEntity.Get<HasOverlayTerrain>().Entity;

    //         types.List.AddRange(overlayTerrain.Get<TerrainTypes>().List);
    //     }

    //     return types;
    // }

    public float GetDefense()
    {
        var defense = 0f;

        foreach (var type in List)
        {
            if (Modifiers.Defenses[type] > defense)
            {
                defense = Modifiers.Defenses[type];
            }
        }

        return defense;
    }

    public int GetMovementCost(Mobility mobility)
    {
        var cost = 0;

        foreach (var type in List)
        {
            if (mobility.Dict.ContainsKey(type))
            {
                if (mobility.Dict[type] > cost)
                {
                    cost = mobility.Dict[type];
                }
            }
            else if (Modifiers.MovementCosts[type] > cost)
            {
                cost = Modifiers.MovementCosts[type];
            }
        }

        return cost;
    }

    public override string ToString()
    {
        return string.Join(", ", List);
    }
}
