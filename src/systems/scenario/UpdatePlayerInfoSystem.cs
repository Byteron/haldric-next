using Bitron.Ecs;

public class UpdatePlayerInfoSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        if (!world.TryGetResource<Scenario>(out var scenario))
        {
            return;
        }

        if (!world.TryGetResource<SidePanel>(out var sidePanel))
        {
            return;
        }

        if (scenario.Side == -1)
        {
            return;
        }

        var sideEntity = scenario.GetCurrentSideEntity();

        var side = sideEntity.Get<Side>().Value;
        ref var playerId = ref sideEntity.Get<PlayerId>();
        ref var gold = ref sideEntity.Get<Gold>();
        ref var name = ref sideEntity.Get<Name>();

        var unitCount = 0;
        var villageCount = 0;
        var capturedVillageCount = 0;
        var income = 0;

        world.ForEach((EcsEntity locEntity, ref Village village) =>
        {
            villageCount += 1;
        });

        world.ForEach((EcsEntity locEntity, ref Village village, ref IsCapturedBySide captured) =>
        {
            if (captured.Value == side)
            {
                capturedVillageCount += 1;
                income += village.List.Count;
            }
        });

        world.ForEach((EcsEntity unitEntity, ref Side unitSide, ref Level level, ref Attribute<Health> health) =>
        {
            if (unitSide.Value == side)
            {
                income -= level.Value;
                unitCount += 1;
            }
        });

        var localPlayer = world.GetResource<LocalPlayer>();

        var roundString = "Round: " + scenario.Round;
        var unitString = "Units: " + unitCount;
        var villageString = $"Villages: {capturedVillageCount} / {villageCount}";
        var youString = $"You: {localPlayer.Presence.Username}";
        var otherString = $"Current: ({side}) {name.Value}";
        var goldString = "Gold: - | Income: -";

        if (localPlayer.Id == playerId.Value)
        {
            goldString = $"Gold: {gold.Value} | Income: {income}";
        }

        sidePanel.UpdateInfo($"{youString} | {roundString} | {otherString} | {goldString} | {unitString} | {villageString}");
    }
}