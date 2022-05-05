using RelEcs;
using RelEcs.Godot;
using Godot;
using Haldric.Wdk;

public class UpdateUnitPlateSystem : ISystem
{
    public void Run(Commands commands)
    {
        commands.ForEach((
            Entity entity,
            ref Side side,
            ref Attribute<Health> health,
            ref Attribute<Moves> moves,
            ref Attribute<Experience> experience,
            ref Node<UnitView> view,
            ref Node<UnitPlate> plate) =>
        {
            plate.Value.MaxHealth = health.Max;
            plate.Value.Health = health.Value;

            plate.Value.MaxMoves = moves.Max;
            plate.Value.Moves = moves.Value;

            plate.Value.MaxExperience = experience.Max;
            plate.Value.Experience = experience.Value;

            plate.Value.Position = view.Value.Position + Vector3.Up * 7.5f;

            plate.Value.SideColor = Data.Instance.SideColors[side.Value];

            plate.Value.IsLeader = entity.Has<IsLeader>();
            plate.Value.IsHero = entity.Has<IsHero>();
        });
    }
}