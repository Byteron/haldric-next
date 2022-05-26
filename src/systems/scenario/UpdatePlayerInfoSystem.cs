using RelEcs;
using RelEcs.Godot;

public class UpdatePlayerInfoSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!commands.TryGetElement<Scenario>(out var scenario)) return;
        if (!commands.TryGetElement<SidePanel>(out var sidePanel)) return;
        if (scenario.Side == -1) return;

        var sideEntity = scenario.GetCurrentSideEntity();

        var side = sideEntity.Get<Side>().Value;
        var playerId = sideEntity.Get<PlayerId>();
        var gold = sideEntity.Get<Gold>();
        var name = sideEntity.Get<Name>();

        var unitCount = 0;
        var villageCount = 0;
        var capturedVillageCount = 0;
        var income = 0;

        foreach (var _ in commands.Query<Village>())
        {
            villageCount += 1;
        }
        
        foreach (var (village, captured) in commands.Query<Village, IsCapturedBySide>())
        {
            if (captured.Value != side) continue;
            
            capturedVillageCount += 1;
            income += village.List.Count;
        }

        foreach (var (unitSide, level) in commands.Query<Side, Level>())
        {
            if (unitSide.Value != side) continue;
            income -= level.Value;
            unitCount += 1;

        }

        var localPlayer = commands.GetElement<LocalPlayer>();

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