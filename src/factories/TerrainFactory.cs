using System.Collections.Generic;
using Bitron.Ecs;
using Haldric.Wdk;

public class TerrainFactory
{
    private static TerrainBuilder _builder = new TerrainBuilder();

    public static EcsEntity CreateFromDict(Dictionary<string, object> dict)
    {
        if (dict.ContainsKey(nameof(IsBaseTerrain)))
        {
            _builder.CreateBase();
        }

        if (dict.ContainsKey(nameof(IsOverlayTerrain)))
        {
            _builder.CreateOverlay();
        }

        if (dict.ContainsKey(nameof(TerrainCode)))
        {
            _builder.WithCode((string)dict[nameof(TerrainCode)]);
        }

        if (dict.ContainsKey(nameof(TerrainTypes)))
        {
            _builder.WithTypes((List<TerrainType>)dict[nameof(TerrainTypes)]);
        }

        if (dict.ContainsKey(nameof(ElevationOffset)))
        {
            _builder.WithElevationOffset((float)dict[nameof(ElevationOffset)]);
        }

        if (dict.ContainsKey(nameof(CanRecruitFrom)))
        {
            _builder.WithRecruitFrom();
        }

        if (dict.ContainsKey(nameof(CanRecruitTo)))
        {
            _builder.WithRecruitTo();
        }

        if (dict.ContainsKey(nameof(GivesIncome)))
        {
            _builder.WithGivesIncome();
        }

        if (dict.ContainsKey(nameof(IsCapturable)))
        {
            _builder.WithIsCapturable();
        }

        if (dict.ContainsKey(nameof(Heals)))
        {
            _builder.WithHeals();
        }

        if (dict.ContainsKey(nameof(NoLighting)))
        {
            _builder.WithNoLighting();
        }

        return _builder.Build();
    }
}