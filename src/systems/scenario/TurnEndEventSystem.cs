using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public struct TurnEndEvent { }

public class TurnEndEventSystem : IEcsSystem
{
    private int _turn = 0;

    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<TurnEndEvent>().End();
        var unitQuery = world.Query<Side>().Inc<Attribute<Actions>>().Inc<Level>().End();
        var locsWithCapturedVillagesQuery = world.Query<Village>().Inc<IsCapturedByTeam>().End();
        var locWithUnitQuery = world.Query<HasBaseTerrain>().Inc<HasUnit>().End();

        foreach (var eventEntityId in eventQuery)
        {
            var scenario = world.GetResource<Scenario>();

            scenario.EndTurn();

            if (_turn != scenario.Turn)
            {
                _turn = scenario.Turn;

                ChangeDaytime(world);
            }

            var player = scenario.GetCurrentPlayerEntity();

            ref var gold = ref player.Get<Gold>();

            foreach (var unitEntityId in unitQuery)
            {
                var unitEntity = world.Entity(unitEntityId);

                ref var side = ref unitEntity.Get<Side>();

                if (side.Value == scenario.CurrentPlayer)
                {
                    ref var actions = ref unitEntity.Get<Attribute<Actions>>();
                    ref var moves = ref unitEntity.Get<Attribute<Moves>>();

                    actions.Restore();
                    moves.Restore();

                    ref var level = ref unitEntity.Get<Level>();

                    gold.Value -= level.Value;
                }
            }

            foreach (var locEntityId in locsWithCapturedVillagesQuery)
            {
                var locEntity = world.Entity(locEntityId);

                ref var village = ref locEntity.Get<Village>();
                ref var side = ref locEntity.Get<IsCapturedByTeam>();

                if (scenario.CurrentPlayer == side.Value)
                {
                    gold.Value += village.List.Count;
                }
            }

            var hudView = world.GetResource<HUDView>();
            var localPlayer = world.GetResource<LocalPlayer>();

            if (scenario.CurrentPlayer == localPlayer.Side)
            {
                hudView.TurnEndButton.Disabled = false;
            }
            else
            {
                hudView.TurnEndButton.Disabled = true;
            }

            foreach (var locEntityId in locWithUnitQuery)
            {
                var locEntity = world.Entity(locEntityId);

                var canHeal = locEntity.Get<HasBaseTerrain>().Entity.Has<Heals>();

                if (locEntity.Has<HasOverlayTerrain>())
                {
                    canHeal = canHeal || locEntity.Get<HasOverlayTerrain>().Entity.Has<Heals>();
                }

                var unitEntity = locEntity.Get<HasUnit>().Entity;

                ref var health = ref unitEntity.Get<Attribute<Health>>();
                ref var side = ref unitEntity.Get<Side>();

                if (canHeal && side.Value == scenario.CurrentPlayer && !health.IsFull())
                {
                    var diff = Mathf.Min(health.GetDifference(), 8);

                    health.Increase(diff);

                    hudView.SpawnFloatingLabel(unitEntity.Get<Coords>().World() + Godot.Vector3.Up * 7f, diff.ToString(), new Godot.Color(0f, 1f, 0f));
                }
            }
        }
    }

    private static void ChangeDaytime(EcsWorld world)
    {
        var schedule = world.GetResource<Schedule>();

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