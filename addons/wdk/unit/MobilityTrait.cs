using Bitron.Ecs;
using Godot;

namespace Haldric.Wdk
{
    public partial class MobilityTrait : Trait
    {
        [Export] TerrainType TerrainType = TerrainType.Flat;
        [Export] int Cost = 1;

        public override void Apply(EcsEntity unitEntity)
        {
            ref var mobility = ref unitEntity.Get<Mobility>();
            mobility.Dict.Add(TerrainType, Cost);
        }
    }
}
