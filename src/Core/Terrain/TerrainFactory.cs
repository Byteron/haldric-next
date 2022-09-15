using System.Collections.Generic;
using RelEcs;

public class TerrainFactory
{
    public static Entity CreateFromDict(World world, Dictionary<string, object> dict)
    {
        var entity = world.Spawn();
        var builder = new EntityBuilder(world, entity.Identity);

        if (dict.ContainsKey(nameof(IsBaseTerrain)))
        {
            builder.Add<IsBaseTerrain>();
        }

        if (dict.ContainsKey(nameof(IsOverlayTerrain)))
        {
            builder.Add<IsOverlayTerrain>();
        }

        if (dict.ContainsKey(nameof(TerrainCode)))
        {
            builder.Add(new TerrainCode { Value = (string)dict[nameof(TerrainCode)] });
        }

        if (dict.ContainsKey(nameof(TerrainTypes)))
        {
            builder.Add(new TerrainTypes { List = (List<TerrainType>)dict[nameof(TerrainTypes)] });
        }

        if (dict.ContainsKey(nameof(ElevationOffset)))
        {
            builder.Add(new ElevationOffset { Value = (float)dict[nameof(ElevationOffset)] });
        }

        if (dict.ContainsKey(nameof(CanRecruitFrom)))
        {
            builder.Add<CanRecruitFrom>();
        }

        if (dict.ContainsKey(nameof(CanRecruitTo)))
        {
            builder.Add<CanRecruitTo>();
        }

        if (dict.ContainsKey(nameof(GivesIncome)))
        {
            builder.Add<GivesIncome>();
        }

        if (dict.ContainsKey(nameof(IsCapturable)))
        {
            builder.Add<IsCapturable>();
        }

        if (dict.ContainsKey(nameof(Heals)))
        {
            builder.Add<Heals>();
        }

        if (dict.ContainsKey(nameof(NoLighting)))
        {
            builder.Add<NoLighting>();
        }

        return entity;
    }
}