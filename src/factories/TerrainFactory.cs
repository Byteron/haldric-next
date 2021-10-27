using System.Collections.Generic;
using Bitron.Ecs;
using Haldric.Wdk;

public class TerrainFactory
{
    private static TerrainBuilder _builder = new TerrainBuilder();

    public static EcsEntity CreateFromDict(Dictionary<string, object> dict)
    {
        if (dict.ContainsKey("BaseTerrain"))
        {
            _builder.CreateBase();
        }

        if (dict.ContainsKey("OverlayTerrain"))
        {
            _builder.CreateOverlay();
        }

        if (dict.ContainsKey("TerrainCode"))
        {
            _builder.WithCode((string)dict["TerrainCode"]);
        }

        if (dict.ContainsKey("TerrainTypes"))
        {
            _builder.WithTypes((List<TerrainType>)dict["TerrainTypes"]);
        }

        if (dict.ContainsKey("HasShallowWater"))
        {
            _builder.WithHasShallowWater();
        }

        if (dict.ContainsKey("HasDeepWater"))
        {
            _builder.WithHasDeepWater();
        }

        if (dict.ContainsKey("RecruitFrom"))
        {
            _builder.WithRecruitFrom();
        }

        if (dict.ContainsKey("RecruitTo"))
        {
            _builder.WithRecruitTo();
        }

        if (dict.ContainsKey("GivesIncome"))
        {
            _builder.WithGivesIncome();
        }

        if (dict.ContainsKey("IsCapturable"))
        {
            _builder.WithIsCapturable();
        }

        if (dict.ContainsKey("Heals"))
        {
            _builder.WithHeals();
        }

        return _builder.Build();
    }
}