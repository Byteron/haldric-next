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

        public void Set(int index)
        {
            _current = index;
            SetVisuals();
        }

        public Daytime GetCurrentDaytime()
        {
            return _daytimes.GetChild<Daytime>(_current);
        }

        public int GetCurrentDaytimeIndex()
        {
            return _current;
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
            tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "ground_horizon_color", daytime.SkyHorizonColor, 2.5f);

            for (int i = 0; i < _lights.Count; i++)
            {
                var light = _lights[i];
                var config = configs[i];

                var lightTween = Main.Instance.GetTree().CreateTween();

                lightTween.SetTrans(Tween.TransitionType.Sine);
                lightTween.SetEase(Tween.EaseType.InOut);

                GD.Print(light.Rotation.x);
                var currentAngle = Mathf.Rad2Deg(light.Rotation.x);
                var angle = config.Angle;

                GD.Print($"Angle: {angle} Current Angle: {currentAngle}");
                if (angle < currentAngle)
                {
                    angle += 360;
                    GD.Print($"Setting Angle: {angle}");
                }

                lightTween.Parallel().TweenProperty(light, "rotation", new Vector3(Mathf.Deg2Rad(angle), 0, 0), 2.5f);
                lightTween.Parallel().TweenProperty(light, "light_color", config.LightColor, 2.5f);
                lightTween.Parallel().TweenProperty(light, "light_energy", config.LightEnergy, 2.5f);

                lightTween.Parallel().TweenProperty(light, "shadow_enabled", config.Shadows, 2.5f);
                lightTween.Parallel().TweenProperty(light, "shadow_color", config.ShadowColor, 2.5f);
                lightTween.Parallel().TweenProperty(light, "shadow_blur", config.ShadowBlur, 2.5f);

                lightTween.Connect("finished", new Callable(this, nameof(SetLightAngle)), new Array() { i, config.Angle });

                lightTween.Play();
            }

            tween.Play();
        }

        private void SetVisuals()
        {
            var daytime = GetCurrentDaytime();
            var configs = daytime.GetLightConfigs();

            GD.Print("Daytime: ", daytime.Name);

            _env.Environment.Sky.SkyMaterial.Set("sky_energy", daytime.SkyEnergy);
            _env.Environment.Sky.SkyMaterial.Set("sky_curve", daytime.SkyCurve);
            _env.Environment.Sky.SkyMaterial.Set("sky_top_color", daytime.SkyTopColor);
            _env.Environment.Sky.SkyMaterial.Set("sky_horizon_color", daytime.SkyHorizonColor);

            for (int i = 0; i < _lights.Count; i++)
            {
                var light = _lights[i];
                var config = configs[i];

                light.Rotation = new Vector3(Mathf.Deg2Rad(config.Angle), 0, 0);
                light.LightColor = config.LightColor;
                light.LightEnergy = config.LightEnergy;
                light.ShadowEnabled = config.Shadows;
                light.ShadowColor = config.ShadowColor;
                light.ShadowBlur = config.ShadowBlur;
            }
        }

        private void SetLightAngle(int index, int angle)
        {
            GD.Print("Resetting Angle: ", angle);
            _lights[index].Rotation = new Vector3(Mathf.Deg2Rad(angle), 0, 0);
        }
    }
}
