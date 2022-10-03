using RelEcs;
using Godot;

public class TurnEndTrigger { }

public class TurnEndSystem : ISystem
{
    public World World { get; set; }

    public void Run(World world)
    {
        foreach (var _ in world.Receive<TurnEndTrigger>(this))
        {
            var scenario = world.GetElement<Scenario>();

            scenario.EndTurn();

            if (scenario.HasRoundChanged())
            {
                var schedule = world.GetElement<Schedule>();
                schedule.Next();
            }

            var sides = world.Query<Gold, Side, PlayerId>();

            var sideEntity = scenario.GetCurrentSideEntity();

            var (gold, side, playerId) = sides.Get(sideEntity);

            var units = world.Query<Entity, Side, Actions, Moves, Level>();
            var suspends = world.QueryBuilder().Has<Suspended>().Build();

            foreach (var (unitEntity, unitSide, actions, moves, level) in units)
            {
                if (side.Value != unitSide.Value) continue;

                actions.Restore();
                moves.Restore();

                if (suspends.Has(unitEntity))
                {
                    world.On(unitEntity).Remove<Suspended>();
                }

                gold.Value -= level.Value;
            }

            foreach (var (village, villageSide) in world.Query<Village, IsCapturedBySide>())
            {
                if (side.Value == villageSide.Value)
                {
                    gold.Value += village.List.Count;
                }
            }

            if (world.TryGetElement<TurnPanel>(out var turnPanel))
            {
                var localPlayer = world.GetElement<LocalPlayer>();

                if (playerId.Value == localPlayer.Id)
                {
                    Sfx.Instance.Play("TurnBell");
                    turnPanel.EndTurnButton.Disabled = false;
                }
                else
                {
                    turnPanel.EndTurnButton.Disabled = true;
                }
            }

            var tiles = world.Query<Entity, BaseTerrainSlot, OverlayTerrainSlot, UnitSlot>();
            var heals = world.QueryBuilder().Has<Heals>().Build();

            var healthsAndSides = world.Query<Coords, Health, Side>();

            foreach (var (tileEntity, baseTerrain, overlayTerrain, unit) in tiles)
            {
                var canHeal = heals.Has(baseTerrain.Entity);

                if (world.IsAlive(overlayTerrain.Entity))
                {
                    canHeal = canHeal || heals.Has(overlayTerrain.Entity);
                }

                var (coords, health, unitSide) = healthsAndSides.Get(unit.Entity);

                if (!canHeal || unitSide.Value != side.Value || health.IsFull()) continue;

                var diff = Mathf.Min(health.GetDifference(), 8);

                health.Increase(diff);

                world.SpawnFloatingLabel(coords.ToWorld() + Godot.Vector3.Up * 7f, diff.ToString(), Colors.Green);
            }
        }
    }
}