using System.Collections.Generic;
using Godot;

namespace Haldric.Wdk
{
    public partial class Daytime : Node
    {
        [Export] public Color SkyTopColor = new Color("FFFFFF");
        [Export] public Color SkyHorizonColor = new Color("FFFFFF");
        [Export(PropertyHint.ExpEasing)] public float SkyCurve = 1f;
        [Export(PropertyHint.Range, "0,100")] public float SkyEnergy = 1f;

        [Export] public List<Alignment> Bonuses;
        [Export] public List<Alignment> Maluses;

        private List<DaytimeLightConfig> _lightConfigs = new List<DaytimeLightConfig>();

        public override void _Ready()
        {
            foreach (Node node in GetChildren())
            {
                if (node is DaytimeLightConfig config)
                {
                    _lightConfigs.Add(config);
                }
            }
        }

        public List<DaytimeLightConfig> GetLightConfigs()
        {
            return _lightConfigs;
        }

        public float GetDamageModifier(Alignment alignment)
        {
            float mod = 1f;

            if (Bonuses != null && Bonuses.Contains(alignment))
            {
                mod += 0.25f;
            }

            if (Maluses != null && Maluses.Contains(alignment))
            {
                mod -= 0.25f;
            }

            return mod;
        }
    }
}
