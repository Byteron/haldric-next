using Godot;
using System.Collections.Generic;

public partial class Schedule : Node
{
    int _current;

    [Export] Node _daytimes;
    [Export] WorldEnvironment _env;
    [Export] Node3D _lightContainer;
    List<DirectionalLight3D> _lights = new();

    public override void _Ready()
    {
        foreach (var child in _lightContainer.GetChildren())
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

    void TweenVisuals()
    {
        var daytime = GetCurrentDaytime();
        var configs = daytime.GetLightConfigs();

        GD.Print("Daytime: ", daytime.Name);

        var tween = GetTree().CreateTween();

        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.InOut);

        tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_energy_multiplier", daytime.SkyEnergy, 2.5f);
        tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_curve", daytime.SkyCurve, 2.5f);
        tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_top_color", daytime.SkyTopColor, 2.5f);
        tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "sky_horizon_color", daytime.SkyHorizonColor, 2.5f);
        tween.Parallel().TweenProperty(_env.Environment.Sky.SkyMaterial, "ground_horizon_color",
            daytime.SkyHorizonColor, 2.5f);

        for (var i = 0; i < _lights.Count; i++)
        {
            var light = _lights[i];
            var config = configs[i];

            var lightTween = GetTree().CreateTween();

            lightTween.SetTrans(Tween.TransitionType.Sine);
            lightTween.SetEase(Tween.EaseType.InOut);

            GD.Print(light.Rotation.X);
            var currentAngle = Mathf.RadToDeg(light.Rotation.X);
            var angle = config.Angle;

            GD.Print($"Angle: {angle} Current Angle: {currentAngle}");
            if (angle < currentAngle)
            {
                angle += 360;
                GD.Print($"Setting Angle: {angle}");
            }

            lightTween.Parallel().TweenProperty(light, "rotation", new Vector3(Mathf.DegToRad(angle), 0, 0), 2.5f);
            lightTween.Parallel().TweenProperty(light, "light_color", config.LightColor, 2.5f);
            lightTween.Parallel().TweenProperty(light, "light_energy", config.LightEnergy, 2.5f);

            lightTween.Parallel().TweenProperty(light, "shadow_enabled", config.Shadows, 2.5f);
            lightTween.Parallel().TweenProperty(light, "shadow_blur", config.ShadowBlur, 2.5f);

            lightTween.Finished += () =>
            {
                GD.Print("Resetting Angle: ", config.Angle);
                light.Rotation = new Vector3(Mathf.DegToRad(config.Angle), 0, 0);
            };

            lightTween.Play();
        }

        tween.Play();
    }

    void SetVisuals()
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

            light.Rotation = new Vector3(Mathf.DegToRad(config.Angle), 0, 0);
            light.LightColor = config.LightColor;
            light.LightEnergy = config.LightEnergy;
            light.ShadowEnabled = config.Shadows;
            // light.ShadowColor = config.ShadowColor;
            light.ShadowBlur = config.ShadowBlur;
        }
    }
}