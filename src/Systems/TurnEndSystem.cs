using RelEcs;
using Godot;

public class TurnEndTrigger { }

public class TurnEndSystem : ISystem
{
    public World World { get; set; }

    public void Run()
    {
        foreach (var _ in this.Receive<TurnEndTrigger>())
        {
            var scenario = this.GetElement<Scenario>();

            scenario.EndTurn();

            if (scenario.HasRoundChanged())
            {
                var schedule = this.GetElement<Schedule>();
                schedule.Next();
            }

            var sides = this.Query<Gold, Side, PlayerId>();

            var sideEntity = scenario.GetCurrentSideEntity();

            var (gold, side, playerId) = sides.Get(sideEntity);

            var units = this.Query<Entity, Side, Actions, Moves, Level>();
            var suspends = this.QueryBuilder().Has<Suspended>().Build();

            foreach (var (unitEntity, unitSide, actions, moves, level) in units)
            {
                if (side.Value != unitSide.Value) continue;

                actions.Restore();
                moves.Restore();

                if (suspends.Has(unitEntity))
                {
                    this.On(unitEntity).Remove<Suspended>();
                }

                gold.Value -= level.Value;
            }

            foreach (var (village, villageSide) in this.Query<Village, IsCapturedBySide>())
            {
                if (side.Value == villageSide.Value)
                {
                    gold.Value += village.List.Count;
                }
            }

            if (this.TryGetElement<TurnPanel>(out var turnPanel))
            {
                var localPlayer = this.GetElement<LocalPlayer>();

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

            var tiles = this.Query<Entity, BaseTerrainSlot, OverlayTerrainSlot, UnitSlot>();
            var heals = this.QueryBuilder().Has<Heals>().Build();

            var healthsAndSides = this.Query<Coords, Health, Side>();

            foreach (var (tileEntity, baseTerrain, overlayTerrain, unit) in tiles)
            {
                var canHeal = heals.Has(baseTerrain.Entity);

                if (this.IsAlive(overlayTerrain.Entity))
                {
                    canHeal = canHeal || heals.Has(overlayTerrain.Entity);
                }

                var (coords, health, unitSide) = healthsAndSides.Get(unit.Entity);

                if (!canHeal || unitSide.Value != side.Value || health.IsFull()) continue;

                var diff = Mathf.Min(health.GetDifference(), 8);

                health.Increase(diff);

                this.SpawnFloatingLabel(coords.ToWorld() + Godot.Vector3.Up * 7f, diff.ToString(), Colors.Green);
            }
        }
    }
}