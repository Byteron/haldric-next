namespace Haldric.Wdk
{
    public class Modifiers
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
        Sandy,
        Aqueous,
        Cavernous,
        Settled,
        Fortified,
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