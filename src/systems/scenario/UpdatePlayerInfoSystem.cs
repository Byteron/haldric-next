using Bitron.Ecs;

public class UpdatePlayerInfoSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.TryGetResource<Scenario>(out var scenario))
        {
            return;
        }

        if (!world.TryGetResource<HUDView>(out var hudView))
        {
            return;
        }

        if (scenario.CurrentPlayer == -1)
        {
            return;
        }
        
        var player = scenario.GetCurrentPlayerEntity();

        ref var team = ref player.Get<Team>();
        ref var gold = ref player.Get<Gold>();

        hudView.PlayerLabel.Text = $"Player: {team.Value} | Gold: {gold.Value}";
    }
}