using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public class UpdateUnitPlateSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        world.ForEach((
            EcsEntity entity,
            ref Side side,
            ref Attribute<Health> health,
            ref Attribute<Moves> moves,
            ref Attribute<Experience> experience,
            ref NodeHandle<UnitView> view,
            ref NodeHandle<UnitPlate> plate) =>
        {
            plate.Node.MaxHealth = health.Max;
            plate.Node.Health = health.Value;

            plate.Node.MaxMoves = moves.Max;
            plate.Node.Moves = moves.Value;

            plate.Node.MaxExperience = experience.Max;
            plate.Node.Experience = experience.Value;

            plate.Node.Position = view.Node.Position + Vector3.Up * 7.5f;

            plate.Node.SideColor = Data.Instance.SideColors[side.Value];

            plate.Node.IsLeader = entity.Has<IsLeader>();
            plate.Node.IsHero = entity.Has<IsHero>();
        });
    }
}