using Godot;
using Godot.Collections;
using System.Collections.Generic;
namespace Haldric.Wdk
{
    public partial class Schedule : Node
    {
        private int _current = 0;

        private Node _daytimes;
        private WorldEnvironment _env;
        private Node3D _lightContainer;
        private List<DirectionalLight3D> _lights = new List<DirectionalLight3D>();

        public override void _Ready()
        {
            _daytimes = GetNode<Node>("Daytimes");
            _env = GetNode<WorldEnvironment>("WorldEnvironment");
            _lightContainer = GetNode<Node3D>("Pivot/LightContainer");

            foreach (Node child in _lightContainer.GetChildren())
            {
                if (child is DirectionalLight3D light)
                {
                    _lights.Add(light);
                }
            }
        }

        public void Next()
        {
            _current = (_current + 1) % _daytimes.GetChildCount();
            TweenVisuals();
        }

        public Daytime GetCurrentDaytime()
        {
            return _daytimes.GetChild<Daytime>(_current);
        }

        private void TweenVisuals()
        {
            var daytime = GetCurrentDaytime();
            var configs = daytime.GetLightConfigs();

            GD.Print("Daytime: ", daytime.Name);

            var tween = Main.Instance.GetTree().CreateTween();

            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.InOut);

            tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_energy", daytime.SkyEnergy, 2.5f);
            tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_curve", daytime.SkyCurve, 2.5f);
            tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_top_color", daytime.SkyTopColor, 2.5f);
            tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_horizon_color", daytime.SkyHorizonColor, 2.5f);

            for (int i = 0; i < _lights.Count; i++)
            {
                var light = _lights[i];
                var config = configs[i];

                var lightTween = Main.Instance.GetTree().CreateTween();

                lightTween.SetTrans(Tween.TransitionType.Sine);
                lightTween.SetEase(Tween.EaseType.InOut);

                var angle = config.Angle;
                if (angle == 0)
                {
                    angle = 360;
                }

                lightTween.Parallel().TweenProperty(light, "rotation", new Vector3(Mathf.Deg2Rad(angle), 0, 0), 2.5f);
                lightTween.Parallel().TweenProperty(light, "light_color", config.LightColor, 2.5f);
                lightTween.Parallel().TweenProperty(light, "light_energy", config.LightEnergy, 2.5f);

                lightTween.Parallel().TweenProperty(light, "shadow_enabled", config.Shadows, 2.5f);
                lightTween.Parallel().TweenProperty(light, "shadow_color", config.ShadowColor, 2.5f);
                lightTween.Parallel().TweenProperty(light, "shadow_blur", config.ShadowBlur, 2.5f);

                if (angle == 360)
                {
                    lightTween.Connect(nameof(lightTween.Finished), new Callable(this, nameof(ResetLight)), new Array() { i });
                }

                lightTween.Play();
            }

            tween.Play();
        }

        private void ResetLight(int index)
        {
            _lights[index].Rotation = new Vector3(0, 0, 0);
        }
    }
}
