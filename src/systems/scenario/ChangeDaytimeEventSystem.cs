using Bitron.Ecs;
using Haldric.Wdk;
using Godot;

public struct ChangeDaytimeEvent { }

public class ChangeDaytimeEventSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<ChangeDaytimeEvent>().End();

        if (!world.TryGetResource<Schedule>(out var schedule))
        {
            return;
        }

        foreach (var eventId in query)
        {
            schedule.Next();

            var daytime = schedule.GetCurrentDaytime();

            var tween = Main.Instance.GetTree().CreateTween();

            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.InOut);

            tween.Parallel().TweenProperty(Main.Instance.Light, "rotation", new Vector3(Mathf.Deg2Rad(daytime.Angle), 0, 0), 2.5f);
            tween.Parallel().TweenProperty(Main.Instance.Light, "light_color", daytime.LightColor, 2.5f);
            tween.Parallel().TweenProperty(Main.Instance.Light, "light_energy", daytime.LightIntensity, 2.5f);

            tween.Parallel().TweenProperty(Main.Instance.Environment.Environment.Sky.SkyMaterial, "sky_energy", daytime.SkyIntensity, 2.5f);
            tween.Parallel().TweenProperty(Main.Instance.Environment.Environment.Sky.SkyMaterial, "sky_top_color", daytime.SkyColor, 2.5f);
            tween.Parallel().TweenProperty(Main.Instance.Environment.Environment.Sky.SkyMaterial, "sky_horizon_color", daytime.SkyColor, 2.5f);
            tween.Parallel().TweenProperty(Main.Instance.Environment.Environment.Sky.SkyMaterial, "ground_horizon_color", daytime.SkyColor, 2.5f);

            tween.Play();            
        }
    }
}