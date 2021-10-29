using System.Collections.Generic;

namespace Haldric.Wdk
{
    public enum DamageType
    {
        Slash,
        Pierce,
        Impact,
        Natural,
        Heat,
        Cold,
        Arcane,
    }

    public enum TerrainType
    {
        Flat,
        Settled,
        Fortified,
        Forested,
        Infested,
        Rough,
        Rocky,
        Reefy,
        ShallowWaters,
        DeepWaters,
        Unwalkable,
        Impassable,
    }

    public static class Modifiers
    {
        public const float Weakness = 1.25f;
        public const float Resistance = 0.75f;
        public const float Calamity = 2f;
        public const float Immunity = 0f;

        public static readonly Dictionary<TerrainType, int> MovementCosts = new Dictionary<TerrainType, int>()
        {
            { TerrainType.Flat, 1 },
            { TerrainType.Settled, 1 },
            { TerrainType.Fortified, 1 },
            { TerrainType.Forested, 2 },
            { TerrainType.Infested, 2 },
            { TerrainType.Rough, 3 },
            { TerrainType.Rocky, 3 },
            { TerrainType.ShallowWaters, 3 },
            { TerrainType.DeepWaters, 99 },
            { TerrainType.Unwalkable, 99 },
            { TerrainType.Impassable, 99 },
        };

        public static readonly Dictionary<TerrainType, float> Defenses = new Dictionary<TerrainType, float>()
        {
            { TerrainType.Flat, 0f },
            { TerrainType.Settled, 0.3f },
            { TerrainType.Fortified, 0.5f },
            { TerrainType.Forested, 0.3f },
            { TerrainType.Infested, 0.3f },
            { TerrainType.Rough, 0.0f },
            { TerrainType.Rocky, 0.4f },
            { TerrainType.ShallowWaters, 0f },
            { TerrainType.DeepWaters, 0f },
            { TerrainType.Unwalkable, 0f },
            { TerrainType.Impassable, 0f },
        };
    }
}