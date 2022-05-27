using System.Collections.Generic;
using RelEcs;
using RelEcs.Godot;
using Haldric.Wdk;

public class TerrainFactory
{
    public static Entity CreateFromDict(Commands commands, Dictionary<string, object> dict)
    {
        var builder = new TerrainBuilder(commands);

        if (dict.ContainsKey(nameof(IsBaseTerrain)))
        {
            builder.CreateBase();
        }

        if (dict.ContainsKey(nameof(IsOverlayTerrain)))
        {
            builder.CreateOverlay();
        }

        if (dict.ContainsKey(nameof(TerrainCode)))
        {
            builder.WithCode((string)dict[nameof(TerrainCode)]);
        }

        if (dict.ContainsKey(nameof(TerrainTypes)))
        {
            builder.WithTypes((List<TerrainType>)dict[nameof(TerrainTypes)]);
        }

        if (dict.ContainsKey(nameof(ElevationOffset)))
        {
            builder.WithElevationOffset((float)dict[nameof(ElevationOffset)]);
        }

        if (dict.ContainsKey(nameof(CanRecruitFrom)))
        {
            builder.WithRecruitFrom();
        }

        if (dict.ContainsKey(nameof(CanRecruitTo)))
        {
            builder.WithRecruitTo();
        }

        if (dict.ContainsKey(nameof(GivesIncome)))
        {
            builder.WithGivesIncome();
        }

        if (dict.ContainsKey(nameof(IsCapturable)))
        {
            builder.WithIsCapturable();
        }

        if (dict.ContainsKey(nameof(Heals)))
        {
            builder.WithHeals();
        }

        if (dict.ContainsKey(nameof(NoLighting)))
        {
            builder.WithNoLighting();
        }

        return builder.Build();
    }
}