using Bitron.Ecs;
using Godot;

public struct TurnEndEvent { }

public class TurnEndEventSystem : IEcsSystem
{
    private int _turn = 0;

    public void Run(EcsWorld world)
    {
        var eventQuery = world.Query<TurnEndEvent>().End();
        var unitQuery = world.Query<Team>().Inc<Attribute<Moves>>().Inc<Attribute<Actions>>().End();
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

            foreach (var unitEntityId in unitQuery)
            {
                var team = unitQuery.Get<Team>(unitEntityId);
                if (team.Value == scenario.CurrentPlayer)
                {
                    ref var actions = ref unitQuery.Get<Attribute<Actions>>(unitEntityId);
                    ref var moves = ref unitQuery.Get<Attribute<Moves>>(unitEntityId);
                    actions.Restore();
                    moves.Restore();
                }
            }

            foreach (var locEntityId in locsWithCapturedVillagesQuery)
            {
                ref var village = ref locsWithCapturedVillagesQuery.Get<Village>(locEntityId);
                ref var team = ref locsWithCapturedVillagesQuery.Get<IsCapturedByTeam>(locEntityId);

                if (scenario.CurrentPlayer == team.Value)
                {
                    player.Get<Gold>().Value += village.List.Count;
                    GD.Print($"Player: {team}, Income + {village.List.Count}");
                }
            }

            var hudView = world.GetResource<HUDView>();

            foreach (var locEntityId in locWithUnitQuery)
            {
                var locEntity = world.Entity(locEntityId);

                var canHeal = locEntity.Get<HasBaseTerrain>().Entity.Has<Heals>();

                if (locEntity.Has<HasOverlayTerrain>())
                {
                    canHeal = canHeal || locEntity.Get<HasOverlayTerrain>().Entity.Has<Heals>();
                }
                
                var unitEntity = locWithUnitQuery.Get<HasUnit>(locEntityId).Entity;

                ref var health = ref unitEntity.Get<Attribute<Health>>();
                ref var team = ref unitEntity.Get<Team>();

                if (canHeal && team.Value == scenario.CurrentPlayer && !health.IsFull())
                {
                    var diff = Mathf.Min(health.GetDifference(), 8);

                    health.Increase(diff);

                    hudView.SpawnFloatingLabel(unitEntity.Get<Coords>().World + Godot.Vector3.Up * 7f, diff.ToString(), new Godot.Color(0f, 1f, 0f));
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
        tween.TweenProperty(Main.Instance.Light, "light_energy", daytime.Energy, 2.5f);
        tween.Parallel();
        tween.TweenProperty(Main.Instance.Light, "rotation", new Vector3(Mathf.Deg2Rad(daytime.Angle), 0, 0), 2.5f);
        tween.Parallel();
        tween.TweenProperty(Main.Instance.Environment.Environment.Sky.SkyMaterial, "sky_top_color", daytime.Color, 2.5f);
        tween.Parallel();
        tween.TweenProperty(Main.Instance.Environment.Environment.Sky.SkyMaterial, "sky_horizon_color", daytime.Color, 2.5f);
        tween.Parallel();
        tween.TweenProperty(Main.Instance.Environment.Environment.Sky.SkyMaterial, "ground_horizon_color", daytime.Color, 2.5f);
        tween.Play();
    }
}