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
            .Inc<Side>()
            .End();

        foreach (var entityId in query)
        {
            var unitEntity = world.Entity(entityId);

            ref var health = ref unitEntity.Get<Attribute<Health>>();
            ref var moves = ref unitEntity.Get<Attribute<Moves>>();
            ref var experience = ref unitEntity.Get<Attribute<Experience>>();
            ref var side = ref unitEntity.Get<Side>();

            var view = unitEntity.Get<NodeHandle<UnitView>>().Node;
            var unitPlate = unitEntity.Get<NodeHandle<UnitPlate>>().Node;

            unitPlate.MaxHealth = health.Max;
            unitPlate.Health = health.Value;

            unitPlate.MaxMoves = moves.Max;
            unitPlate.Moves = moves.Value;

            unitPlate.MaxExperience = experience.Max;
            unitPlate.Experience = experience.Value;

            unitPlate.Position = view.Position + Vector3.Up * 7.5f;

            unitPlate.SideColor = Data.Instance.SideColors[side.Value];

            unitPlate.IsLeader = unitEntity.Has<IsLeader>();
            unitPlate.IsHero = unitEntity.Has<IsHero>();
        }
    }
}