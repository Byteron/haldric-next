using Godot;

namespace Haldric.Wdk
{
    public partial class DaytimeLightConfig : Node
    {
        [Export(PropertyHint.Range, "0,360")] public float Angle = 0f;
        [Export] public Color LightColor = new Color("FFFFFF");
        [Export(PropertyHint.Range, "0,100")] public float LightEnergy = 1f;

        [Export] public bool Shadows = true;
        [Export] public Color ShadowColor = new Color("000000");
        [Export(PropertyHint.Range, "0,100")] public float ShadowBlur = 1f;
    }
}
