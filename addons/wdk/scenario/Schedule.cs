using Godot;
using Godot.Collections;
using System.Collections.Generic;
namespace Haldric.Wdk
{
	public partial class Schedule : Node
	{
		int current;

		Node daytimes;
		WorldEnvironment env;
		Node3D lightContainer;
		List<DirectionalLight3D> lights = new();

		public override void _Ready()
		{
			daytimes = GetNode<Node>("Daytimes");
			env = GetNode<WorldEnvironment>("WorldEnvironment");
			lightContainer = GetNode<Node3D>("Pivot/LightContainer");

			foreach (Node child in lightContainer.GetChildren())
			{
				if (child is DirectionalLight3D light)
				{
					lights.Add(light);
				}
			}
		}

		public void Next()
		{
			current = (current + 1) % daytimes.GetChildCount();
			TweenVisuals();
		}

		public void Set(int index)
		{
			current = index;
			SetVisuals();
		}

		public Daytime GetCurrentDaytime()
		{
			return daytimes.GetChild<Daytime>(current);
		}

		public int GetCurrentDaytimeIndex()
		{
			return current;
		}

		void TweenVisuals()
		{
			var daytime = GetCurrentDaytime();
			var configs = daytime.GetLightConfigs();

			GD.Print("Daytime: ", daytime.Name);

			var tween = GetTree().CreateTween();

			tween.SetTrans(Tween.TransitionType.Sine);
			tween.SetEase(Tween.EaseType.InOut);

			tween.Parallel().TweenProperty(env.Environment.Sky.SkyMaterial, "sky_energy", daytime.SkyEnergy, 2.5f);
			tween.Parallel().TweenProperty(env.Environment.Sky.SkyMaterial, "sky_curve", daytime.SkyCurve, 2.5f);
			tween.Parallel().TweenProperty(env.Environment.Sky.SkyMaterial, "sky_top_color", daytime.SkyTopColor, 2.5f);
			tween.Parallel().TweenProperty(env.Environment.Sky.SkyMaterial, "sky_horizon_color", daytime.SkyHorizonColor, 2.5f);
			tween.Parallel().TweenProperty(env.Environment.Sky.SkyMaterial, "ground_horizon_color", daytime.SkyHorizonColor, 2.5f);

			for (int i = 0; i < lights.Count; i++)
			{
				var light = lights[i];
				var config = configs[i];

				var lightTween = GetTree().CreateTween();

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
				// lightTween.Parallel().TweenProperty(light, "shadow_color", config.ShadowColor, 2.5f);
				lightTween.Parallel().TweenProperty(light, "shadow_blur", config.ShadowBlur, 2.5f);

				lightTween.Connect("finished", new Callable(this, nameof(SetLightAngle)), new Array() { i, config.Angle });

				lightTween.Play();
			}

			tween.Play();
		}

		void SetVisuals()
		{
			var daytime = GetCurrentDaytime();
			var configs = daytime.GetLightConfigs();

			GD.Print("Daytime: ", daytime.Name);

			env.Environment.Sky.SkyMaterial.Set("sky_energy", daytime.SkyEnergy);
			env.Environment.Sky.SkyMaterial.Set("sky_curve", daytime.SkyCurve);
			env.Environment.Sky.SkyMaterial.Set("sky_top_color", daytime.SkyTopColor);
			env.Environment.Sky.SkyMaterial.Set("sky_horizon_color", daytime.SkyHorizonColor);

			for (int i = 0; i < lights.Count; i++)
			{
				var light = lights[i];
				var config = configs[i];

				light.Rotation = new Vector3(Mathf.Deg2Rad(config.Angle), 0, 0);
				light.LightColor = config.LightColor;
				light.LightEnergy = config.LightEnergy;
				light.ShadowEnabled = config.Shadows;
				// light.ShadowColor = config.ShadowColor;
				light.ShadowBlur = config.ShadowBlur;
			}
		}

		void SetLightAngle(int index, int angle)
		{
			GD.Print("Resetting Angle: ", angle);
			lights[index].Rotation = new Vector3(Mathf.Deg2Rad(angle), 0, 0);
		}
	}
}
