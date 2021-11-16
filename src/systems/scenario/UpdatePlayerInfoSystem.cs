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

        if (!world.TryGetResource<HudView>(out var hudView))
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
        ref var playerName = ref player.Get<Id>();

        var locWithCapturedVillageQuery = world.Query<Village>().End();
        var unitQuery = world.Query<Side>().Inc<Attribute<Actions>>().Inc<Level>().End();

        var unitCount = 0;
        var villageCount = 0;
        var capturedVillageCount = 0;
        var income = 0;

        foreach (var locId in locWithCapturedVillageQuery)
        {
            var locEntity = world.Entity(locId);

            villageCount += 1;

            if (!locEntity.Has<IsCapturedBySide>())
            {
                continue;
            }

            ref var captured = ref locEntity.Get<IsCapturedBySide>();

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
                unitCount += 1;
            }
        }

        var localPlayer = world.GetResource<LocalPlayer>();

        var turnString = "Turn: " + scenario.Turn;
        var unitString = "Units: " + unitCount;
        var villageString = $"Villages: {capturedVillageCount} / {villageCount}";
        var youString = $"You: ({localPlayer.Side}) {localPlayer.Presence.Username}";
        var otherString = $"Current: ({playerSide.Value}) {playerName.Value}";

        if (localPlayer.Side != playerSide.Value)
        {
            hudView.PlayerLabel.Text = $"{youString} | {turnString} | {otherString} | Gold: - | {unitString} | {villageString}";
        }
        else
        {
            hudView.PlayerLabel.Text = $"{youString} | {turnString} | {otherString} | Gold: {playerGold.Value} | {unitString} | {villageString}";
        }
    }
}