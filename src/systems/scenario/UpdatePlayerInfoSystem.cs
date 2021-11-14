using System.Linq;
using Bitron.Ecs;
using Nakama;

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
        
        ref var playerSide = ref player.Get<Side>();
        ref var playerGold = ref player.Get<Gold>();

        var locWithCapturedVillageQuery = world.Query<Village>().End();
        var unitQuery = world.Query<Side>().Inc<Attribute<Actions>>().Inc<Level>().End();

        var villageCount = 0;
        var capturedVillageCount = 0;
        var income = 0;

        foreach (var locId in locWithCapturedVillageQuery)
        {
            var locEntity = world.Entity(locId);
            
            villageCount += 1;

            if (!locEntity.Has<IsCapturedByTeam>())
            {
                continue;
            }

            ref var captured = ref locEntity.Get<IsCapturedByTeam>();

            if (captured.Value == playerSide.Value)
            {
                ref var village = ref locEntity.Get<Village>();
                
                capturedVillageCount += 1;
                income += village.List.Count;
            }
        }

        foreach (var unitEntityId in unitQuery)
        {
            var unitEntity = world.Entity(unitEntityId);

            ref var unitSide = ref unitEntity.Get<Side>();

            if (unitSide.Value == playerSide.Value)
            {
                ref var level = ref unitEntity.Get<Level>();   
                income -= level.Value;
            }
        }

        if (world.TryGetResource<LocalPlayer>(out var localPlayer))
        {
            if (localPlayer.Side != playerSide.Value)
            {
                hudView.PlayerLabel.Text = $"Turn: {scenario.Turn} | Player: {playerSide.Value} | Gold: - | Villages: - / {villageCount}";
            }
            else
            {
                hudView.PlayerLabel.Text = $"Turn: {scenario.Turn} | Player: {playerSide.Value} | Gold: {playerGold.Value} ({income}) | Villages: {capturedVillageCount} / {villageCount}";
            }
        }
        else
        {
            hudView.PlayerLabel.Text = $"Turn: {scenario.Turn} | Player: {playerSide.Value} | Gold: {playerGold.Value} ({income}) | Villages: {capturedVillageCount} / {villageCount}";
        }

    }
}