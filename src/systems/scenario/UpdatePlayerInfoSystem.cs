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

        ref var side = ref player.Get<Side>();
        ref var gold = ref player.Get<Gold>();

        hudView.PlayerLabel.Text = $"Turn: {scenario.Turn} | Player: {side.Value} | Gold: {gold.Value}";
    }
}