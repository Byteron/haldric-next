using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;

public class UpdateUnitPlateSystem : ISystem
{
    public void Run(Commands commands)
    {
        var query = commands
            .Query<Side, Attribute<Health>, Attribute<Moves>, Attribute<Experience>, UnitView, UnitPlate>();

        foreach (var (side, health, moves, experience, view, plate) in query)
        {
            plate.MaxHealth = health.Max;
            plate.Health = health.Value;

            plate.MaxMoves = moves.Max;
            plate.Moves = moves.Value;

            plate.MaxExperience = experience.Max;
            plate.Experience = experience.Value;

            plate.Position = view.Position + Vector3.Up * 7.5f;

            plate.SideColor = Data.Instance.SideColors[side.Value];

            plate.IsLeader = false;
            plate.IsHero = false;
        }

        foreach (var plate in commands.Query<UnitPlate>().Has<IsLeader>())
        {
            plate.IsLeader = true;
        }
        
        foreach (var plate in commands.Query<UnitPlate>().Has<IsHero>())
        {
            plate.IsHero = true;
        }
    }
}