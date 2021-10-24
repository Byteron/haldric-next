namespace Haldric.Wdk
{
    public static class Modifiers
    {
        public const float Weakness = 1.25f;
        public const float Resistance = 0.75f;
        public const float Calamity = 2f;
        public const float Immunity = 0f;
    }

    public enum TerrainType
    {
        Flat,
        Forested,
        Rough,
        Rocky,
        Aqueous,
        Oceanic,
        Infested,
        Settled,
        Fortified,
        Unwalkable,
        Impassable,
    }

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
}