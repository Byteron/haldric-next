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

        ref var side = ref sideEntity.Get<Side>();
        ref var playerId = ref sideEntity.Get<PlayerId>();
        ref var gold = ref sideEntity.Get<Gold>();
        ref var name = ref sideEntity.Get<Name>();

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

            if (captured.Value == side.Value)
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

            if (unitSide.Value == side.Value)
            {
                ref var level = ref unitEntity.Get<Level>();
                income -= level.Value;
                unitCount += 1;
            }
        }

        var localPlayer = world.GetResource<LocalPlayer>();

        var roundString = "Round: " + scenario.Round;
        var unitString = "Units: " + unitCount;
        var villageString = $"Villages: {capturedVillageCount} / {villageCount}";
        var youString = $"You: {localPlayer.Presence.Username}";
        var otherString = $"Current: ({side.Value}) {name.Value}";
        var goldString = "Gold: - | Income: -";

        if (localPlayer.Id == playerId.Value)
        {
            goldString = $"Gold: {gold.Value} | Income: {income}";
        }

        sidePanel.UpdateInfo($"{youString} | {roundString} | {otherString} | {goldString} | {unitString} | {villageString}");
    }
}