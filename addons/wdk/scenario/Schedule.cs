using Godot;
using System;

namespace Haldric.Wdk
{
    public partial class Schedule : Node
    {   
        private int _current = 0;

        private Node _daytimes;
        private WorldEnvironment _env;
        private DirectionalLight3D _light;

        public override void _Ready()
        {
            _daytimes = GetNode<Node>("Daytimes");
            _env = GetNode<WorldEnvironment>("WorldEnvironment");
            _light = GetNode<DirectionalLight3D>("Pivot/LightContainer/DirectionalLight3D");
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

            GD.Print("Daytime: ", daytime.Name);
            
            var tween = Main.Instance.GetTree().CreateTween();

            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.InOut);

            var angle = daytime.Angle;
            if (angle == 0)
            {
                angle = 360;
            }

            tween.Parallel().TweenProperty(_light, "rotation", new Vector3(Mathf.Deg2Rad(angle), 0, 0), 2.5f);
            tween.Parallel().TweenProperty(_light, "light_color", daytime.LightColor, 2.5f);
            tween.Parallel().TweenProperty(_light, "light_energy", daytime.LightIntensity, 2.5f);

            tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_energy", daytime.SkyIntensity, 2.5f);
            tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_top_color", daytime.SkyColor, 2.5f);
            tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_horizon_color", daytime.SkyColor, 2.5f);
            tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "ground_horizon_color", daytime.SkyColor, 2.5f);
            
            if (angle == 360)
            {
                tween.TweenCallback(new Callable(this, nameof(ResetLight)));
            }

            tween.Play();
        }

        private void ResetLight()
        {
            _light.Rotation = new Vector3(0, 0, 0);
        }
    }
}
