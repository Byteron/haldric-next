using System.Collections.Generic;
using Bitron.Ecs;
using Haldric.Wdk;

public struct TerrainTypes
{
    public List<TerrainType> List;

    public TerrainTypes(List<TerrainType> list)
    {
        List = list;
    }

	public static TerrainTypes FromLocEntity(EcsEntity locEntity)
    {
        var types = new TerrainTypes(new List<TerrainType>());

        var baseTerrain = locEntity.Get<HasBaseTerrain>().Entity;

        types.List.AddRange(baseTerrain.Get<TerrainTypes>().List);

        if (locEntity.Has<HasOverlayTerrain>())
        {
            var overlayTerrain = locEntity.Get<HasOverlayTerrain>().Entity;

            types.List.AddRange(overlayTerrain.Get<TerrainTypes>().List);
        }

        return types;
    }

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

	public int GetMovementCost()
	{
		var cost = 0;

        foreach (var type in List)
        {
            if (Modifiers.Defenses[type] > cost)
            {
                cost = Modifiers.MovementCosts[type];
            }
        }

		return cost;
	}

	public override string ToString()
	{
		string s = "";

		for (int i = 0; i < List.Count; i++)
		{
			s += List[i].ToString();

			if (i < List.Count - 1)
			{
				s += ", ";
			}
			
		}
		foreach (var type in List)
		{
		}

		s += "";
		return s;
	}
}