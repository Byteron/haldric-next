using System.Collections.Generic;
using Godot;

namespace Haldric.Wdk
{
    public partial class Daytime : Node
    {
        [Export] public float Angle = 0f;
        [Export] public float SkyEnergy = 1f;
        [Export] public float LightEnergy = 1f;
        [Export] public Color SkyColor = new Color("FFFFFF");
        [Export] public Color LightColor = new Color("FFFFFF");

        [Export] public List<Alignment> Bonuses;
        [Export] public List<Alignment> Maluses;

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
