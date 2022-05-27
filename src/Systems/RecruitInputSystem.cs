using RelEcs;
using RelEcs.Godot;
using Godot;

public class RecruitInputSystem : ISystem
{
    public void Run(Commands commands)
    {
        if (!Input.IsActionJustPressed("recruit")) return;
        
        var scenario = commands.GetElement<Scenario>();
        var localPlayer = commands.GetElement<LocalPlayer>();
        var sideEntity = scenario.GetCurrentSideEntity();
        var side = sideEntity.Get<Side>();
        var playerId = sideEntity.Get<PlayerId>();

        if (localPlayer.Id != playerId.Value) return;

        var canRecruit = false;
        Entity targetCastleLocEntity = null;

        var castleQuery = commands.Query<Entity>().Has<Castle>();
        
        foreach (var castleLocEntity in castleQuery)
        {
            if (!castleLocEntity.Has<HasUnit>()) continue;

            var castleUnitEntity = castleLocEntity.Get<HasUnit>().Entity;

            if (!castleUnitEntity.Has<IsLeader>()) continue;

            if (castleUnitEntity.Get<Side>().Value != side.Value) continue;

            targetCastleLocEntity = castleLocEntity;
            canRecruit = true;
            break;
        }

        if (!canRecruit) return;

        Entity freeLocEntity;

        var castle = targetCastleLocEntity.Get<Castle>();

        var hLocEntity = commands.GetElement<HoveredLocation>().Entity;

        if (hLocEntity.IsAlive && castle.IsLocFree(hLocEntity.Get<Coords>())) freeLocEntity = hLocEntity;
        else if (!castle.TryGetFreeLoc(out freeLocEntity)) return;

        var gameStateController = commands.GetElement<GameStateController>();

        var recruitState = new RecruitSelectionState();
        recruitState.FreeLocEntity = freeLocEntity;
        gameStateController.PushState(recruitState);
    }
}