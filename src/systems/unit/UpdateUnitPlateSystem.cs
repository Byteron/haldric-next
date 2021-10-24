using Bitron.Ecs;
using Godot;
using Haldric.Wdk;

public class UpdateUnitPlateSystem : IEcsSystem
{
    public void Run(EcsWorld world)
    {
        var query = world.Query<NodeHandle<UnitView>>()
            .Inc<Attribute<Health>>()
            .Inc<Attribute<Moves>>()
            .Inc<Attribute<Experience>>()
            .Inc<NodeHandle<UnitPlate>>()
            .Inc<Team>()
            .End();

        foreach (var entityId in query)
        {
            ref var health = ref query.Get<Attribute<Health>>(entityId);
            ref var moves = ref query.Get<Attribute<Moves>>(entityId);
            ref var experience = ref query.Get<Attribute<Experience>>(entityId);
            ref var team = ref query.Get<Team>(entityId);

            var view = query.Get<NodeHandle<UnitView>>(entityId).Node;
            var unitPlate = query.Get<NodeHandle<UnitPlate>>(entityId).Node;

            unitPlate.MaxHealth = health.Max;
            unitPlate.Health = health.Value;

            unitPlate.MaxMoves = moves.Max;
            unitPlate.Moves = moves.Value;

            unitPlate.MaxExperience = experience.Max;
            unitPlate.Experience = experience.Value;

            unitPlate.Position = view.Position + Vector3.Up * 7.5f;

            unitPlate.TeamColor = Data.Instance.TeamColors[team.Value];
        }
    }
}